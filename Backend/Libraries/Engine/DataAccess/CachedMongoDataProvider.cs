using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;
using Microsoft.Extensions.Caching.Memory;

namespace GaiaProject.Engine.DataAccess
{
	public class CachedMongoDataProvider : IProvideData
	{
		private readonly IMemoryCache _memoryCache;
		private readonly MongoDataProvider _mongoProvider;

		public CachedMongoDataProvider(IMemoryCache memoryCache, MongoDataProvider mongoProvider)
		{
			_memoryCache = memoryCache;
			_mongoProvider = mongoProvider;
		}

		private string Key<T>(string id) => $"{typeof(T).FullName}_{id}";

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

		public void FlushGame(string id)
		{
			var key = Key<GaiaProjectGame>(id);
			this.Flush(key);
		}

		public void FlushUser(User user)
		{
			var keys = new List<string>
			{
				Key<User>(user.Id),
				Key<User>($"_auth0_identifier_{user.Identifier}"),
				Key<User>($"_username_{user.Username}"),
			};
			keys.ForEach(key => this.Flush(key));
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

		public Task<string> CreateGame(GaiaProjectGame game)
		{
			return _mongoProvider.CreateGame(game);
		}

		public async Task<GaiaProjectGame> GetGame(string id)
		{
			return await Get(id, () => this._mongoProvider.GetGame(id));
		}

		public Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId)
		{
			return this._mongoProvider.GetInitialGameState(gameId);
		}

		public async Task<User> GetUser(string id)
		{
			return await Get(id, () => this._mongoProvider.GetUser(id));
		}

		public async Task<User> GetUserByIdentifier(string identifier)
		{
			return await Get($"_auth0_identifier_{identifier}", () => this._mongoProvider.GetUserByIdentifier(identifier));
		}

		public async Task<User> GetUserByUsername(string username)
		{
			return await Get($"_username_{username}", () => this._mongoProvider.GetUserByUsername(username));
		}

		public async Task<string> GetUsername(string userId)
		{
			return (await GetUser(userId))?.Username;
		}

		public Task<User[]> GetUsers(Expression<Func<User, bool>> predicate)
		{
			return this._mongoProvider.GetUsers(predicate);
		}

		public Task<User[]> GetAllUsers()
		{
			return this._mongoProvider.GetAllUsers();
		}

		public async Task<string> CreateUser(User user)
		{
			return await _mongoProvider.CreateUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _mongoProvider.UpdateUser(user);
			this.FlushUser(user);
		}

		public Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true)
		{
			return this._mongoProvider.GetUserGames(userId, onlyActive);
		}

		public async Task SaveGame(GaiaProjectGame game)
		{
			await this._mongoProvider.SaveGame(game);
			this.FlushGame(game.Id);
		}
	}
}