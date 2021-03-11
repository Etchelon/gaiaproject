﻿using System;
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

		public async Task<List<Notification>> GetUserNotifications(string userId, DateTime earlierThan)
		{
			return await this._userDataProvider.GetUserNotifications(userId, earlierThan, 10);
		}
	}
}
