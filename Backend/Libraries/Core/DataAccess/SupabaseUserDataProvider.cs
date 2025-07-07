using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Core.DataAccess.Abstractions;
using GaiaProject.Core.Model;

namespace GaiaProject.Core.DataAccess
{
	public class SupabaseUserDataProvider : IProvideUserData
	{
		private readonly Supabase.Client _client;

		public SupabaseUserDataProvider(Supabase.Client client)
		{
			_client = client;
		}

		public async Task<User> GetUser(string id)
		{
			var guid = Guid.TryParse(id, out var parsedId) ? parsedId : Guid.Empty;
            return await _client.From<User>().Where(u => u.Id == guid).Single();
        }

		public async Task<User> GetUserByUsername(string username)
		{
            return await _client.From<User>().Where(u => u.Username == username).Single();
        }

        public async Task<string> GetUsername(string userId)
		{
			return (await GetUser(userId)).Username;
		}

		public async Task<User[]> GetUsers(Expression<Func<User, bool>> predicate)
		{
			return (await _client.From<User>().Where(predicate).Get()).Models.ToArray();
		}

		public async Task<User[]> GetAllUsers()
		{
            return (await _client.From<User>().Get()).Models.ToArray();
        }

		public async Task<long> CountUnreadNotifications(string userId)
		{
			return await Task.FromResult(0);
		}

		public async Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan, int pageSize)
		{
            return await Task.FromResult(new List<Notification>());
        }

        public async Task<List<Notification>> GetUserNotificationsByGame(string userId, string gameId)
		{
            return await Task.FromResult(new List<Notification>());
        }

        public async Task SetNotificationRead(string notificationId)
		{
		}

		public async Task SetNotificationsRead(IEnumerable<string> notificationIds)
		{
		}

		public async Task<string> CreateUserNotification(Notification notification)
		{
			return "notification.Id";
		}
	}
}
