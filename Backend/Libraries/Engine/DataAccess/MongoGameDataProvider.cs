﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Common.Database;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;
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