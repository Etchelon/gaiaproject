using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GaiaProject.Common.Reflection;
using GaiaProject.Core.Logic;
using GaiaProject.Endpoint.Hubs;
using GaiaProject.Endpoint.Mapping.Resolvers;
using GaiaProject.Endpoint.Utils;
using GaiaProject.Engine.Commands;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.ViewModels;
using GaiaProject.ViewModels.Actions;
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
				NotifyPlayersAsync(game);
			}
			return game.Id;
		}

		internal async Task<HandleActionResult> HandleAction(string gameId, string userId, JToken action)
		{
			var actualAction = GetActualAction(action);
			actualAction.PlayerId = userId;
			actualAction.PlayerUsername = await _userManager.GetUsername(userId);
			var result = await _gameManager.HandleAction(gameId, actualAction);
			if (result.Handled)
			{
				NotifyPlayersAsync(result.NewState);
			}
			return result;
		}

		#region Helpers

		private PlayerAction GetActualAction(JToken action)
		{
			var actionType = (ActionType)action.Value<int>(nameof(ActionViewModel.Type));
			var phase = actionType.GetAttributeOfType<AvailableInPhaseAttribute>().Phase;
			return phase == GamePhase.Setup
				? GetSetupAction(action)
				: GetRoundsAction(action);
		}

		private PlayerAction GetSetupAction(JToken action)
		{
			var type = action.Value<int>(nameof(ActionViewModel.Type));
			var typeEnum = (ActionType)type;
			switch (typeEnum)
			{
				default:
					throw new NotImplementedException();
				case ActionType.SelectRace:
					{
						var actionVm = action.ToObject<SelectRaceActionViewModel>();
						return new SelectRaceAction
						{
							Race = actionVm.Race,
						};
					}
				case ActionType.BidForRace:
					{
						var actionVm = action.ToObject<BidForRaceActionViewModel>();
						return new BidForRaceAction
						{
							Race = actionVm.Race,
							Points = actionVm.Points,
						};
					}
				case ActionType.PlaceInitialStructure:
					{
						var actionVm = action.ToObject<PlaceInitialStructureActionViewModel>();
						return new PlaceInitialStructureAction
						{
							TargetHexId = actionVm.HexId,
						};
					}
				case ActionType.SelectStartingRoundBooster:
					{
						var actionVm = action.ToObject<SelectRoundBoosterActionViewModel>();
						return new SelectStartingRoundBoosterAction
						{
							Booster = actionVm.Booster,
						};
					}
			}
		}

		private PlayerAction GetRoundsAction(JToken action)
		{
			var type = action.Value<int>(nameof(ActionViewModel.Type));
			var typeEnum = (ActionType)type;
			switch (typeEnum)
			{
				default:
					throw new NotImplementedException();
				case ActionType.PassTurn:
					{
						return new PassTurnAction();
					}
				case ActionType.ColonizePlanet:
					{
						var actionVm = action.ToObject<ColonizePlanetActionViewModel>();
						return new ColonizePlanetAction
						{
							TargetHexId = actionVm.HexId,
						};
					}
				case ActionType.UpgradeExistingStructure:
					{
						var actionVm = action.ToObject<UpgradeExistingStructureActionViewModel>();
						return new UpgradeExistingStructureAction
						{
							TargetHexId = actionVm.HexId,
							TargetBuildingType = actionVm.TargetBuilding
						};
					}
				case ActionType.ResearchTechnology:
					{
						var actionVm = action.ToObject<ResearchTechnologyActionViewModel>();
						return new ResearchTechnologyAction
						{
							TrackId = actionVm.Track
						};
					}
				case ActionType.ChooseTechnologyTile:
					{
						var actionVm = action.ToObject<ChooseTechnologyTileActionViewModel>();
						return new ChooseTechnologyTileAction
						{
							TileId = actionVm.TileId,
							Advanced = actionVm.Advanced,
							CoveredTileId = (StandardTechnologyTileType?)actionVm.CoveredTileId
						};
					}
				case ActionType.ChargePower:
					{
						var actionVm = action.ToObject<ChargeOrDeclinePowerActionViewModel>();
						return new ChargePowerAction
						{
							Accepted = actionVm.Accepted
						};
					}
				case ActionType.Pass:
					{
						var actionVm = action.ToObject<PassActionViewModel>();
						return new PassAction
						{
							SelectedRoundBooster = actionVm.SelectedRoundBooster,
						};
					}
				case ActionType.Conversions:
					{
						var actionVm = action.ToObject<ConversionsActionViewModel>();
						return new ConversionsAction
						{
							Conversions = actionVm.Conversions
						};
					}
				case ActionType.StartGaiaProject:
					{
						var actionVm = action.ToObject<StartGaiaProjectActionViewModel>();
						return new StartGaiaProjectAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.SortIncomes:
					{
						var actionVm = action.ToObject<SortIncomesActionViewModel>();
						return new SortIncomesAction
						{
							SortedIncomes = actionVm.SortedIncomes
						};
					}
				case ActionType.Power:
					{
						var actionVm = action.ToObject<PowerActionViewModel>();
						return new PowerAction
						{
							ActionId = actionVm.Id
						};
					}
				case ActionType.Qic:
					{
						var actionVm = action.ToObject<QicActionViewModel>();
						return new QicAction
						{
							ActionId = actionVm.Id
						};
					}
				case ActionType.UseTechnologyTile:
					{
						var actionVm = action.ToObject<UseTechnologyTileActionViewModel>();
						return new UseTechnologyTileAction
						{
							TileId = actionVm.TileId,
							Advanced = actionVm.Advanced
						};
					}
				case ActionType.UseRoundBooster:
					{
						return new UseRoundBoosterAction();
					}
				case ActionType.BescodsResearchProgress:
					{
						return new BescodsResearchProgressAction();
					}
				case ActionType.IvitsPlaceSpaceStation:
					{
						var actionVm = action.ToObject<IvitsPlaceSpaceStationActionViewModel>();
						return new IvitsPlaceSpaceStationAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.AmbasSwapPlanetaryInstituteAndMine:
					{
						var actionVm = action.ToObject<AmbasSwapPlanetaryInstituteAndMineActionViewModel>();
						return new AmbasSwapPlanetaryInstituteAndMineAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.FiraksDowngradeResearchLab:
					{
						var actionVm = action.ToObject<FiraksDowngradeResearchLabActionViewModel>();
						return new FiraksDowngradeResearchLabAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.ItarsBurnPowerForTechnologyTile:
					{
						return new ItarsBurnPowerForTechnologyTileAction();
					}
				case ActionType.TerransDecideIncome:
					{
						var actionVm = action.ToObject<TerransDecideIncomeActionViewModel>();
						return new TerransDecideIncomeAction
						{
							Credits = actionVm.Credits,
							Ores = actionVm.Ores,
							Knowledge = actionVm.Knowledge,
							Qic = actionVm.Qic
						};
					}
				case ActionType.UseRightAcademy:
					{
						return new UseRightAcademyAction();
					}
				case ActionType.FormFederation:
					{
						var actionVm = action.ToObject<FormFederationActionViewModel>();
						return new FormFederationAction
						{
							SelectedBuildings = actionVm.SelectedBuildings,
							SelectedSatellites = actionVm.SelectedSatellites,
							SelectedFederationToken = actionVm.SelectedFederationToken
						};
					}
				case ActionType.PlaceLostPlanet:
					{
						var actionVm = action.ToObject<PlaceLostPlanetActionViewModel>();
						return new PlaceLostPlanetAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.RescoreFederationToken:
					{
						var actionVm = action.ToObject<RescoreFederationTokenActionViewModel>();
						return new RescoreFederationTokenAction
						{
							Token = actionVm.Token
						};
					}
				case ActionType.TaklonsLeech:
					{
						var actionVm = action.ToObject<TaklonsLeechActionViewModel>();
						return new TaklonsLeechAction
						{
							Accepted = actionVm.Accepted,
							ChargeFirstThenToken = actionVm.ChargeFirstThenToken
						};
					}
				case ActionType.AcceptOrDeclineLastStep:
					{
						var actionVm = action.ToObject<AcceptOrDeclineLastStepActionViewModel>();
						return new AcceptOrDeclineLastStepAction
						{
							Accepted = actionVm.Accepted,
							Track = actionVm.Track
						};
					}
			}
		}

		private async Task NotifyPlayersAsync(GaiaProjectGame game)
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
				offlineUsers.ForEach(async playerId => await NotifyPlayerByMail(playerId, gameVm));
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
				await NotifyPlayerByMail(activePlayerId, gameVm);
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
			// Intentionally notify players in async way to avoid blocking the user who performed the action
			var auth0Id = (await _userManager.GetUser(id)).Identifier;
			await _hubContext.Clients.User(auth0Id).SendAsync(GaiaHub.GameStateChanged, payload);
		}

		private async Task NotifyPlayerByMail(string activePlayerId, GameStateViewModel gameVm)
		{
			var user = await _userManager.GetUser(activePlayerId);
			var message = _mailHelper.GetEmail(user, gameVm);
			await this._mailService.Send(message);
		}

		private GameStateViewModel MapGameState(GaiaProjectGame game, string userId = null)
		{
			var gameState = _mapper.Map<GameStateViewModel>(game, opt =>
			{
				opt.Items["Game"] = game;
			});
			if (userId != null)
			{
				gameState.ActivePlayer = MapActivePlayer(game, userId);
			}
			return gameState;
		}

		private ActivePlayerInfoViewModel MapActivePlayer(GaiaProjectGame game, string requestPlayerId)
		{
			var activePlayerVm = new ActivePlayerResolver().Resolve(game, requestPlayerId);
			return activePlayerVm;
		}

		#endregion
	}
}