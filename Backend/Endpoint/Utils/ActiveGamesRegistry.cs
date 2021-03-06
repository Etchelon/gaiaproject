using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GaiaProject.Endpoint.Utils
{
	public class ActiveGamesRegistry
	{
		private readonly Dictionary<string, List<string>> GamesAndUsers = new Dictionary<string, List<string>>();

		public ActiveGamesRegistry(ILogger<ActiveGamesRegistry> logger)
		{
			logger.LogWarning($"Service {nameof(ActiveGamesRegistry)} constructed.");
		}

		public List<string> GetUsersInGame(string gameId)
		{
			var hasGameInRegistry = GamesAndUsers.TryGetValue(gameId, out var ret);
			if (!hasGameInRegistry)
			{
				ret = new List<string>();
				GamesAndUsers.Add(gameId, ret);
			}
			return ret;
		}

		public bool GameHasActiveUsers(string gameId)
		{
			return GetUsersInGame(gameId).Any();
		}

		public bool UserIsActiveOnGame(string userId, string gameId)
		{
			return GetUsersInGame(gameId).Contains(userId);
		}

		public void SetUserActiveOnGame(string userId, string gameId)
		{
			if (UserIsActiveOnGame(userId, gameId))
			{
				return;
			}
			GetUsersInGame(gameId).Add(userId);
		}

		public void SetUserInactiveOnGame(string userId, string gameId)
		{
			if (!UserIsActiveOnGame(userId, gameId))
			{
				return;
			}
			GetUsersInGame(gameId).Remove(userId);
		}

		internal void SetUserInactiveOnAllGames(string userId)
		{
			foreach (var (__, users) in GamesAndUsers)
			{
				users.Remove(userId);
			}
		}

		public int CountActiveUsersInGame(string gameId)
		{
			return GetUsersInGame(gameId).Count;
		}
	}
}
