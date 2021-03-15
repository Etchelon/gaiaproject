using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Common.Database;
using GaiaProject.Core.DataAccess.Abstractions;
using GaiaProject.Core.Model;
using MongoDB.Driver;

namespace GaiaProject.Core.DataAccess
{
	public class MongoUserDataProvider : IProvideUserData
	{
		private readonly MongoEntityRepository _repository;

		public MongoUserDataProvider(MongoEntityRepository repository)
		{
			_repository = repository;
		}

		public async Task<User> GetUser(string id)
		{
			return await _repository.GetOneAsync<User>(u => u.Id == id);
		}

		public async Task<User> GetUserByIdentifier(string identifier)
		{
			return await _repository.GetOneAsync<User>(u => u.Identifier == identifier);
		}

		public async Task<User> GetUserByUsername(string username)
		{
			return await _repository.GetOneAsync<User>(u => u.Username == username);
		}

		public async Task<string> GetUsername(string userId)
		{
			return await _repository.ProjectOneAsync<User, string>(userId, u => u.Username);
		}

		public async Task<User[]> GetUsers(Expression<Func<User, bool>> predicate)
		{
			var ret = await _repository.GetAllAsync<User>(predicate);
			return ret.ToArray();
		}

		public async Task<User[]> GetAllUsers()
		{
			var ret = await _repository.GetAllAsync<User>(_ => true);
			return ret.ToArray();
		}

		public async Task<string> CreateUser(User user)
		{
			user.MemberSince = DateTime.Now;
			await _repository.AddOneAsync(user);
			return user.Id;
		}

		public async Task UpdateUser(User user)
		{
			var updateDefinition = Builders<User>.Update.Combine(
				Builders<User>.Update.Set(u => u.Username, user.Username),
				Builders<User>.Update.Set(u => u.FirstName, user.FirstName),
				Builders<User>.Update.Set(u => u.LastName, user.LastName),
				Builders<User>.Update.Set(u => u.Avatar, user.Avatar)
			);
			await _repository.UpdateOneAsync(u => u.Id == user.Id, updateDefinition);
		}

		public async Task<long> CountUnreadNotifications(string userId)
		{
			var filter = Builders<Notification>.Filter.Eq(n => n.TargetUserId, userId);
			filter &= Builders<Notification>.Filter.Eq(n => n.IsRead, false);
			return await _repository.CountAsync(filter);
		}

		public async Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan, int pageSize)
		{
			var filter = Builders<Notification>.Filter.Eq(n => n.TargetUserId, userId);
			filter &= Builders<Notification>.Filter.Lt(n => n.DateCreated, earlierThan);
			var sorter = Builders<Notification>.Sort.Descending(n => n.DateCreated);
			return await _repository.GetPaginatedAsync(filter, sorter, 0, pageSize);
		}

		public async Task SetNotificationRead(string notificationId)
		{
			var updateDefinition = Builders<Notification>.Update.Set(n => n.IsRead, true);
			await _repository.UpdateOneAsync(n => n.Id == notificationId, updateDefinition);
		}

		public async Task<string> CreateUserNotification(Notification notification)
		{
			await _repository.AddOneAsync(notification);
			return notification.Id;
		}
	}
}
