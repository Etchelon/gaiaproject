using System;
using System.Threading.Tasks;
using AutoMapper;
using GaiaProject.Endpoint.Utils;
using GaiaProject.Engine.DataAccess;
using Microsoft.AspNetCore.SignalR;

namespace GaiaProject.Endpoint.Hubs
{
	public class GaiaHub : ApplicationBaseHub
	{
		#region Clientside Invokeable Methods

		public const string GameStateChanged = nameof(GameStateChanged);
		public const string SetOnlineUsers = nameof(SetOnlineUsers);
		public const string UserJoinedGame = nameof(UserJoinedGame);
		public const string UserLeftGame = nameof(UserLeftGame);

		#endregion

		private readonly IMapper _mapper;
		private readonly ActiveGamesRegistry _activeGamesRegistry;
		private readonly ActiveUsersRegistry _activeUsersRegistry;
		private readonly CachedMongoDataProvider _cachedMongoDataProvider;

		public GaiaHub(IMapper mapper, ActiveGamesRegistry activeGamesRegistry, ActiveUsersRegistry activeUsersRegistry, CachedMongoDataProvider cachedMongoDataProvider)
		{
			_mapper = mapper;
			_activeGamesRegistry = activeGamesRegistry;
			_activeUsersRegistry = activeUsersRegistry;
			_cachedMongoDataProvider = cachedMongoDataProvider;
		}

		public override Task OnConnectedAsync()
		{
			_activeUsersRegistry.SetUserOnline(User.Id);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has connected");
			return Task.CompletedTask;
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			var userId = User.Id;
			var userGameIds = _activeGamesRegistry.GetUserGames(userId);
			userGameIds.ForEach(async gameId => await RemoveUserFromGame(gameId));
			_activeUsersRegistry.SetUserOffline(userId);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has disconnected");
			return Task.CompletedTask;
		}

		public async Task JoinGame(string gameId)
		{
			var userId = this.User.Id;
			var groupName = GetGroupName(gameId);
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			_activeGamesRegistry.SetUserActiveOnGame(userId, gameId);

			// Notify other players
			var onlineUsers = _activeGamesRegistry.GetUsersInGame(gameId);
			Clients.Caller.SendAsync(SetOnlineUsers, onlineUsers);
			Clients.GroupExcept(groupName, new[] { Context.ConnectionId }).SendAsync(UserJoinedGame, userId);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has connected to game {gameId}");
		}

		public async Task LeaveGame(string gameId)
		{
			await RemoveUserFromGame(gameId);
		}

		private static string GetGroupName(string gameId)
		{
			return $"GP-Game-{gameId}";
		}

		private async Task RemoveUserFromGame(string gameId)
		{
			var userId = User.Id;
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has disconnected from game {gameId}");

			// Notify other users
			var groupName = GetGroupName(gameId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
			Clients.Group(groupName).SendAsync(UserLeftGame, userId);

			// Remove from the cache
			_activeGamesRegistry.SetUserInactiveOnGame(userId, gameId);
			var isEmpty = _activeGamesRegistry.CountActiveUsersInGame(gameId) == 0;
			if (!isEmpty)
			{
				return;
			}
			_cachedMongoDataProvider.FlushGame(gameId);
		}
	}
}