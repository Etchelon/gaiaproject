using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;
using MongoDB.Driver;
using ScoreSheets.Common.Database;

namespace GaiaProject.Engine.DataAccess
{
	public class MongoDataProvider : IProvideData
	{
		private readonly MongoEntityRepository _repository;

		public MongoDataProvider(MongoEntityRepository repository)
		{
			_repository = repository;
		}

		public async Task<string> CreateGame(GaiaProjectGame game)
		{
			await _repository.AddOneAsync(game);

			// Save initial state to be able to time travel
			var initialState = new InitialGaiaProjectGameState
			{
				GameId = game.Id,
				Players = game.Players,
				BoardState = game.BoardState,
				Setup = game.Setup
			};
			await _repository.AddOneAsync(initialState);

			return game.Id;
		}

		public async Task<GaiaProjectGame> GetGame(string id)
		{
			return await _repository.GetOneAsync<GaiaProjectGame>(g => g.Id == id);
		}

		public async Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId)
		{
			return await _repository.GetOneAsync<InitialGaiaProjectGameState>(g => g.GameId == gameId);
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

		public async Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true)
		{
			var filter = Builders<GaiaProjectGame>.Filter.Eq("Players.Id", userId);
			if (onlyActive)
			{
				filter &= Builders<GaiaProjectGame>.Filter.Eq(g => g.Ended, null);
			}
			var games = await _repository.GetAllAsync(filter);
			return games.ToArray();
		}

		public async Task SaveGame(GaiaProjectGame game)
		{
			var isOver = game.Ended.HasValue;
			if (isOver)
			{
				game.Actions = null;
				await _repository.DeleteOneAsync<InitialGaiaProjectGameState>(o => o.GameId == game.Id);
			}
			await _repository.UpdateOneAsync(game);
		}
	}
}
