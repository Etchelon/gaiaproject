using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Core.DataAccess.Abstractions;
using GaiaProject.Core.Model;
using Microsoft.Extensions.Caching.Memory;

namespace GaiaProject.Core.DataAccess
{
	public class CachedMongoUserDataProvider : IProvideUserData
	{
		private readonly IMemoryCache _memoryCache;
		private readonly MongoUserDataProvider _mongoUserProvider;

		public CachedMongoUserDataProvider(IMemoryCache memoryCache, MongoUserDataProvider mongoUserProvider)
		{
			_memoryCache = memoryCache;
			_mongoUserProvider = mongoUserProvider;
		}

		private static string Key<T>(string id) => $"{typeof(T).FullName}_{id}";

		private async Task<T> Get<T>(string id, Func<Task<T>> getter)
		{
			var key = Key<T>(id);
			if (this._memoryCache.TryGetValue<T>(key, out var cachedObj))
			{
				return cachedObj;
			}

			var obj = await getter();
			if (obj != null)
			{
				this._memoryCache.Set(key, obj);
			}
			return obj;
		}

		public void FlushUser(User user)
		{
			var keys = new List<string>
			{
				Key<User>(user.Id),
				Key<User>($"_auth0_identifier_{user.Identifier}"),
				Key<User>($"_username_{user.Username}"),
			};
			keys.ForEach(this.Flush);
		}

		private void Flush<T>(string id)
		{
			var key = Key<T>(id);
			this.Flush(key);
		}

		private void Flush(string key)
		{
			this._memoryCache.Remove(key);
		}

		public async Task<User> GetUser(string id)
		{
			return await Get(id, () => this._mongoUserProvider.GetUser(id));
		}

		public async Task<User> GetUserByIdentifier(string identifier)
		{
			return await Get($"_auth0_identifier_{identifier}", () => this._mongoUserProvider.GetUserByIdentifier(identifier));
		}

		public async Task<User> GetUserByUsername(string username)
		{
			return await Get($"_username_{username}", () => this._mongoUserProvider.GetUserByUsername(username));
		}

		public async Task<string> GetUsername(string userId)
		{
			return (await GetUser(userId))?.Username;
		}

		public Task<User[]> GetUsers(Expression<Func<User, bool>> predicate)
		{
			return this._mongoUserProvider.GetUsers(predicate);
		}

		public Task<User[]> GetAllUsers()
		{
			return this._mongoUserProvider.GetAllUsers();
		}

		public async Task<string> CreateUser(User user)
		{
			return await _mongoUserProvider.CreateUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _mongoUserProvider.UpdateUser(user);
			this.FlushUser(user);
		}

		#region TODO: implement caching for notifications

		private static string UserNotificationsKey(string userId) => $"_user_{userId}_notifications";

		public async Task<long> CountUnreadNotifications(string userId)
		{
			return await _mongoUserProvider.CountUnreadNotifications(userId);
		}

		public async Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan, int pageSize)
		{
			return await _mongoUserProvider.GetUserNotifications(userId, earlierThan, pageSize);
		}

		public async Task SetNotificationRead(string notificationId)
		{
			await _mongoUserProvider.SetNotificationRead(notificationId);
		}

		public async Task<string> CreateUserNotification(Notification notification)
		{
			return await this._mongoUserProvider.CreateUserNotification(notification);
		}

		#endregion
	}
}