using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic
{
	public class UserManager
	{
		private readonly IProvideData _dataProvider;

		public UserManager(IProvideData dataProvider)
		{
			_dataProvider = dataProvider;
		}

		public async Task<User> GetUser(string userId)
		{
			var user = await _dataProvider.GetUser(userId);
			return user;
		}

		public async Task<User> GetUserByIdentifier(string auth0Id)
		{
			var user = await _dataProvider.GetUserByIdentifier(auth0Id);
			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			var user = await _dataProvider.GetUserByUsername(username);
			return user;
		}

		public async Task<string> GetUsername(string userId)
		{
			return await this._dataProvider.GetUsername(userId);
		}

		public async Task<List<User>> GetUsers(Expression<Func<User, bool>> predicate)
		{
			return (await _dataProvider.GetUsers(predicate)).ToList();
		}

		public async Task<User[]> GetAllUsers()
		{
			return await _dataProvider.GetAllUsers();
		}

		public async Task<bool> UserExists(string id)
		{
			return (await _dataProvider.GetUser(id)) != null;
		}

		public async Task<bool> Auth0UserExists(string identifier)
		{
			return (await _dataProvider.GetUserByIdentifier(identifier)) != null;
		}

		public async Task<string> CreateUser(User user)
		{
			return await _dataProvider.CreateUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _dataProvider.UpdateUser(user);
		}
	}
}
