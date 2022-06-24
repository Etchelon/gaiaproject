using System;
using System.Linq;
using System.Threading.Tasks;
using GaiaProject.Common.Database;
using GaiaProject.Core.Model;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Model;
using MongoDB.Driver;

namespace GaiaProject.Engine.DataAccess
{
	public class MongoGameDataProvider : IProvideGameData
	{
		private readonly MongoEntityRepository _repository;

		public MongoGameDataProvider(MongoEntityRepository repository)
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

		public async Task<GaiaProjectGame[]> GetAllGames(bool active, int page, int pageSize)
		{
			var filter = active
				? Builders<GaiaProjectGame>.Filter.Eq(g => g.Ended, null)
				: Builders<GaiaProjectGame>.Filter.Ne(g => g.Ended, null);
			var sorter = Builders<GaiaProjectGame>.Sort.Descending(o => o.Created);
			var skip = page * pageSize;
			var games = await _repository.GetPaginatedAsync(filter, sorter, skip, pageSize);
			return games.ToArray();
		}

		public async Task<long> CountAllGames(bool active)
		{
			var filter = active
				? Builders<GaiaProjectGame>.Filter.Eq(g => g.Ended, null)
				: Builders<GaiaProjectGame>.Filter.Ne(g => g.Ended, null);
			var count = await _repository.CountAsync(filter);
			return count;
		}

		public async Task SaveGame(GaiaProjectGame game)
		{
			var isOver = game.Ended.HasValue;
			// TODO: move the following code out of here and switch to a message driven architecture
			if (isOver)
			{
				game.Actions = null;
				await _repository.DeleteOneAsync<InitialGaiaProjectGameState>(o => o.GameId == game.Id);
				await _repository.DeleteManyAsync<GameNotes>(gn => gn.GameId == game.Id);
				// Delete all previous game notifications
				var filter = Builders<Notification>.Filter.OfType<GameNotification>(gn => gn.GameId == game.Id);
				await _repository.DeleteManyAsync(filter);
			}
			await _repository.UpdateOneAsync(game);
		}

		public async Task<string> GetPlayerNotes(string playerId, string gameId)
		{
			var gameNotes = await _repository.GetOneAsync<GameNotes>(gn => gn.GameId == gameId && gn.UserId == playerId);
			return gameNotes?.Notes;
		}

		public async Task SavePlayerNotes(string playerId, string gameId, string notes)
		{
			var filter = Builders<GameNotes>.Filter.Eq(gn => gn.GameId, gameId) & Builders<GameNotes>.Filter.Eq(gn => gn.UserId, playerId);
			if (await _repository.AnyAsync(filter))
			{
				var update = Builders<GameNotes>.Update.Set(gn => gn.Notes, notes);
				await _repository.UpdateOneAsync(filter, update);
			}
			else
			{
				await _repository.AddOneAsync(new GameNotes
				{
					GameId = gameId,
					UserId = playerId,
					Notes = notes,
				});
			}
		}

		public async Task DeleteGame(string id)
		{
			await _repository.DeleteOneAsync<InitialGaiaProjectGameState>(o => o.GameId == id);
			await _repository.DeleteManyAsync<GaiaProjectGame>(g => g.Id == id);
			// Delete all previous game notifications
			var filter = Builders<Notification>.Filter.OfType<GameNotification>(gn => gn.GameId == id);
		}
	}
}
