using System;
using System.Collections.Generic;

namespace GaiaProject.Endpoint.Utils
{
	public class ActiveUsersRegistry
	{
		private readonly List<string> Users = new List<string>();
		public List<string> OnlineUsers => new List<string>(Users);

		public ActiveUsersRegistry()
		{
			Console.WriteLine($"Service {nameof(ActiveUsersRegistry)} constructed.");
		}

		public bool UserIsOnline(string userId)
		{
			return Users.Contains(userId);
		}

		public void SetUserOnline(string userId)
		{
			if (UserIsOnline(userId))
			{
				return;
			}
			Users.Add(userId);
		}

		public void SetUserOffline(string userId)
		{
			if (!UserIsOnline(userId))
			{
				return;
			}
			Users.Remove(userId);
		}
	}
}
