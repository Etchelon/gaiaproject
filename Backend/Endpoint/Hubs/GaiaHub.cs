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
		public const string GameStateChanged = nameof(GameStateChanged);
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

		public override async Task OnConnectedAsync()
		{
			_activeUsersRegistry.SetUserOnline(User.Id);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has connected");
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			_activeUsersRegistry.SetUserOffline(User.Id);
			_activeGamesRegistry.SetUserInactiveOnAllGames(User.Id);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has disconnected");
		}

		public async Task JoinGame(string gameId)
		{
			var userId = this.User.Id;
			var groupName = GetGroupName(gameId);
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			_activeGamesRegistry.SetUserActiveOnGame(userId, gameId);
			var onlineUsers = _activeGamesRegistry.GetUsersInGame(gameId);
			Clients.Caller.SendAsync("SetOnlineUsers", onlineUsers);
			Clients.GroupExcept(groupName, new[] { Context.ConnectionId }).SendAsync("UserJoinedGame", userId);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has connected to game {gameId}");
		}

		public async Task LeaveGame(string gameId)
		{
			var userId = User.Id;
			var groupName = GetGroupName(gameId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
			_activeGamesRegistry.SetUserInactiveOnGame(userId, gameId);
			Clients.GroupExcept(groupName, new[] { Context.ConnectionId }).SendAsync("UserLeftGame", userId);
			System.Diagnostics.Debug.WriteLine($"User {User.Username} has disconnected from game {gameId}");
			var isEmpty = _activeGamesRegistry.CountActiveUsersInGame(gameId) == 0;
			if (!isEmpty)
			{
				return;
			}
			_cachedMongoDataProvider.FlushGame(gameId);
		}

		internal static string GetGroupName(string gameId)
		{
			return $"GP-Game-{gameId}";
		}
	}
}