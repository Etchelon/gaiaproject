using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GaiaProject.Core.Logic;
using GaiaProject.Endpoint.Hubs;
using GaiaProject.Endpoint.Mapping.Resolvers;
using GaiaProject.Endpoint.Shared;
using GaiaProject.Endpoint.Utils;
using GaiaProject.Engine.Commands;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Model;
using GaiaProject.ViewModels;
using GaiaProject.ViewModels.Players;
using GaiaProject.ViewModels.Users;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GaiaProject.Endpoint.WorkerServices
{
	public class GamesWorkerService
	{
		private readonly IMapper _mapper;
		private readonly GameManager _gameManager;
		private readonly UserManager _userManager;
		private readonly IHubContext<GaiaHub> _hubContext;
		private readonly ActiveGamesRegistry _activeGamesRegistry;
		private readonly ActiveUsersRegistry _activeUsersRegistry;
		private readonly MailService _mailService;
		private readonly MailHelper _mailHelper;

		public GamesWorkerService(IMapper mapper, GameManager gameManager, UserManager userManager, IHubContext<GaiaHub> hubContext,
			ActiveGamesRegistry activeGamesRegistry, ActiveUsersRegistry activeUsersRegistry, MailService mailService, MailHelper mailHelper)
		{
			_mapper = mapper;
			_gameManager = gameManager;
			_userManager = userManager;
			_hubContext = hubContext;
			_activeGamesRegistry = activeGamesRegistry;
			_activeUsersRegistry = activeUsersRegistry;
			_mailService = mailService;
			_mailHelper = mailHelper;
		}

		internal async Task<GameInfoViewModel[]> GetUserGames(string userId, bool onlyWaitingForAction)
		{
			var games = await _gameManager.GetUserGames(userId);
			if (onlyWaitingForAction)
			{
				games = games.Where(g => g.ActivePlayerId == userId).ToArray();
			}
			return games.Select(game => _mapper.Map<GameInfoViewModel>(game, opt => opt.Items["Game"] = game)).ToArray();
		}

		internal async Task<GameInfoViewModel[]> GetUserFinishedGames(string userId)
		{
			var games = await _gameManager.GetUserFinishedGames(userId);
			return games.Select(game => _mapper.Map<GameInfoViewModel>(game, opt => opt.Items["Game"] = game)).ToArray();
		}

		internal async Task<Page<GameInfoViewModel>> GetAllGames(string kind, int skip, int take)
		{
			var (games, hasMore) = await _gameManager.GetAllGames(kind == "active", skip, take);
			var gameDtos = games.Select(game => _mapper.Map<GameInfoViewModel>(game, opt => opt.Items["Game"] = game)).ToArray();
			return new Page<GameInfoViewModel>
			{
				Items = gameDtos,
				HasMore = hasMore
			};
		}

		internal async Task<GameStateViewModel> GetGame(string id, string userId)
		{
			var game = await _gameManager.GetGame(id);
			return MapGameState(game, userId);
		}

		internal async Task RollbackGameAtAction(string id, int actionId, bool persist)
		{
			var game = await _gameManager.GetGameAtAction(id, actionId, persist);
			NotifyPlayersAsync(game);
		}

		internal async Task<string> GetPlayerNotes(string playerId, string gameId)
		{
			var notes = await _gameManager.GetPlayerNotes(playerId, gameId);
			return notes;
		}

		internal async Task SavePlayerNotes(string playerId, string gameId, string notes)
		{
			await _gameManager.SavePlayerNotes(playerId, gameId, notes);
		}

		internal async Task<UserViewModel[]> SearchUsers(string filter, string username)
		{
			const int minLength = 2;
			if (filter.Length < minLength)
			{
				return new UserViewModel[0];
			}
			var regex = new Regex(filter, RegexOptions.ECMAScript | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			var users = (await _userManager.GetAllUsers())
				.Where(u => u.Username != username && (regex.IsMatch(u.Username) || regex.IsMatch(u.FirstName) || regex.IsMatch(u.LastName)))
				.ToArray();
			return _mapper.Map<UserViewModel[]>(users);
		}

		internal async Task<string> CreateGame(CreateGameCommand command, string createdBy)
		{
			var game = await _gameManager.CreateGame(command, createdBy);
			if (game.ActivePlayerId != createdBy)
			{
				NotifyPlayersAsync(game, NotificationReason.GameStarted);
			}
			return game.Id;
		}

		internal async Task DeleteGame(GameStateViewModel gameVm, string userId)
		{
			await _gameManager.DeleteGame(gameVm.Id);
			gameVm.Players
				.Where(p => p.Id != userId)
				.ToList()
				.ForEach(p => SendDelayedNotifications(p.Id, gameVm, NotificationReason.GameDeleted));
		}

		internal async Task<HandleActionResult> HandleAction(string gameId, string userId, JToken action)
		{
			var actualAction = ActionMapper.GetActualAction(action);
			actualAction.PlayerId = userId;
			actualAction.PlayerUsername = await _userManager.GetUsername(userId);
			var result = await _gameManager.HandleAction(gameId, actualAction);
			if (result.Handled)
			{
				await _userManager.SetNotificationsForGameRead(userId, gameId);
				NotifyPlayersAsync(result.NewState);
			}
			return result;
		}

		#region Helpers

		private async Task NotifyPlayersAsync(GaiaProjectGame game, NotificationReason reason = NotificationReason.YourTurn)
		{
			var gameId = game.Id;
			var gameVm = MapGameState(game);
			var onlineUsers = _activeGamesRegistry.GetUsersInGame(gameId);

			var activePlayerId = game.ActivePlayerId;
			// If no active player it means the game is over
			if (activePlayerId == null)
			{
				var payload = JsonConvert.SerializeObject(
					gameVm,
					new JsonSerializerSettings
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
						TypeNameHandling = TypeNameHandling.Auto
					}
				);

				onlineUsers.ForEach(async playerId => await NotifyPlayerBySignalR(playerId, payload));
				var offlineUsers = game.Players.Where(p => !onlineUsers.Contains(p.Id)).Select(p => p.Id).ToList();
				offlineUsers.ForEach(async playerId => await SendDelayedNotifications(playerId, gameVm, NotificationReason.GameEnded));
				return;
			}

			// Notify active player
			gameVm.ActivePlayer = MapActivePlayer(game, activePlayerId);
			if (onlineUsers.Contains(activePlayerId))
			{
				var payloadForActivePlayer = JsonConvert.SerializeObject(
					gameVm,
					new JsonSerializerSettings
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
						TypeNameHandling = TypeNameHandling.Auto
					}
				);
				await NotifyPlayerBySignalR(activePlayerId, payloadForActivePlayer);
			}
			else
			{
				await SendDelayedNotifications(activePlayerId, gameVm, reason);
			}

			var otherOnlinePlayers = onlineUsers.Where(userId => userId != activePlayerId).ToList();
			if (otherOnlinePlayers.Count == 0)
			{
				return;
			}

			gameVm.ActivePlayer = MapActivePlayer(game, game.Players.First(p => p.Id != activePlayerId).Id);
			var payloadForOtherPlayers = JsonConvert.SerializeObject(
				gameVm,
				new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					TypeNameHandling = TypeNameHandling.Auto
				}
			);

			otherOnlinePlayers.ForEach(async playerId => await NotifyPlayerBySignalR(playerId, payloadForOtherPlayers));
		}

		private async Task NotifyPlayerBySignalR(string id, string payload)
		{
			var auth0Id = (await _userManager.GetUser(id)).Identifier;
			await _hubContext.Clients.User(auth0Id).SendAsync(GaiaHub.GameStateChanged, payload);
		}

		private async Task SendDelayedNotifications(string userId, GameStateViewModel gameVm, NotificationReason reason)
		{
			var user = await _userManager.GetUser(userId);
			var mailMessage = _mailHelper.GetEmail(user, gameVm, reason);
			await this._mailService.Send(mailMessage);
			var notificationMessage = reason switch
			{
				NotificationReason.GameStarted => $"Game {gameVm.Name} has started",
				NotificationReason.YourTurn => $"It's your turn to move in game {gameVm.Name}",
				NotificationReason.GameEnded => $"Game {gameVm.Name} has ended",
				NotificationReason.GameDeleted => $"Game {gameVm.Name} has been deleted",
			};
			if (reason == NotificationReason.GameDeleted)
			{
				await _userManager.NotifyUser(userId, notificationMessage);
				return;
			}
			await _userManager.NotifyUserForGame(userId, gameVm.Id, notificationMessage);
		}

		private GameStateViewModel MapGameState(GaiaProjectGame game, string userId = null)
		{
			var gameState = _mapper.Map<GameStateViewModel>(game, opt =>
			{
				opt.Items["Game"] = game;
			});
			gameState.ActivePlayer = MapActivePlayer(game, userId);
			return gameState;
		}

		private ActivePlayerInfoViewModel MapActivePlayer(GaiaProjectGame game, string? requestPlayerId)
		{
			var activePlayerVm = new ActivePlayerResolver().Resolve(game, requestPlayerId);
			return activePlayerVm;
		}

		#endregion
	}
}