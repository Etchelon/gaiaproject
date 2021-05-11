using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Core.DataAccess.Abstractions;
using GaiaProject.Core.Model;

namespace GaiaProject.Core.Logic
{
	public class UserManager
	{
		private readonly IProvideUserData _userDataProvider;

		public UserManager(IProvideUserData userDataProvider)
		{
			_userDataProvider = userDataProvider;
		}

		public async Task<User> GetUser(string userId)
		{
			var user = await _userDataProvider.GetUser(userId);
			return user;
		}

		public async Task<User> GetUserByIdentifier(string auth0Id)
		{
			var user = await _userDataProvider.GetUserByIdentifier(auth0Id);
			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			var user = await _userDataProvider.GetUserByUsername(username);
			return user;
		}

		public async Task<string> GetUsername(string userId)
		{
			return await this._userDataProvider.GetUsername(userId);
		}

		public async Task<List<User>> GetUsers(Expression<Func<User, bool>> predicate)
		{
			return (await _userDataProvider.GetUsers(predicate)).ToList();
		}

		public async Task<User[]> GetAllUsers()
		{
			return await _userDataProvider.GetAllUsers();
		}

		public async Task<bool> UserExists(string id)
		{
			return (await _userDataProvider.GetUser(id)) != null;
		}

		public async Task<bool> Auth0UserExists(string identifier)
		{
			return (await _userDataProvider.GetUserByIdentifier(identifier)) != null;
		}

		public async Task<string> CreateUser(User user)
		{
			return await _userDataProvider.CreateUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _userDataProvider.UpdateUser(user);
		}

		public async Task<long> CountUnreadNotifications(string userId)
		{
			return await this._userDataProvider.CountUnreadNotifications(userId);
		}

		public async Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan)
		{
			return await this._userDataProvider.GetUserNotifications(userId, earlierThan, 10);
		}

		public async Task SetNotificationRead(string userId, string notificationId)
		{
			// TODO: validation that this notification is targeted towards userId?
			await this._userDataProvider.SetNotificationRead(notificationId);
		}

		public async Task NotifyUser(string userId, string message)
		{
			var notification = new Notification
			{
				DateCreated = DateTime.Now,
				TargetUserId = userId,
				Text = message,
			};
			await this._userDataProvider.CreateUserNotification(notification);
		}

		public async Task NotifyUserForGame(string userId, string gameId, string message)
		{
			var notification = new GameNotification
			{
				DateCreated = DateTime.Now,
				TargetUserId = userId,
				GameId = gameId,
				Text = message,
			};
			await this._userDataProvider.CreateUserNotification(notification);
		}

		public async Task SetNotificationsForGameRead(string userId, string gameId)
		{
			var notifications = await this._userDataProvider.GetUserNotificationsByGame(userId, gameId);
			await this._userDataProvider.SetNotificationsRead(notifications.Select(n => n.Id));
		}
	}
}
