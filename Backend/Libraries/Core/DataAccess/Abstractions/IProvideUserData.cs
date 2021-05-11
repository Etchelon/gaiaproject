using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Core.Model;

namespace GaiaProject.Core.DataAccess.Abstractions
{
	public interface IProvideUserData
	{
		Task<User> GetUser(string id);
		Task<User> GetUserByIdentifier(string identifier);
		Task<User> GetUserByUsername(string username);
		Task<string> GetUsername(string userId);
		Task<User[]> GetUsers(Expression<Func<User, bool>> predicate);
		Task<User[]> GetAllUsers();
		Task<string> CreateUser(User user);
		Task UpdateUser(User user);
		Task<long> CountUnreadNotifications(string userId);
		Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan, int pageSize);
		Task<List<Notification>> GetUserNotificationsByGame(string userId, string gameId);
		Task SetNotificationRead(string notificationId);
		Task SetNotificationsRead(IEnumerable<string> notificationIds);
		Task<string> CreateUserNotification(Notification notification);
	}
}