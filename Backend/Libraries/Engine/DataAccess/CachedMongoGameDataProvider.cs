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
    public class CachedMongoGameDataProvider : IProvideGameData
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MongoGameDataProvider _mongoGameProvider;

        public CachedMongoGameDataProvider(IMemoryCache memoryCache, MongoGameDataProvider mongoGameProvider)
        {
            _memoryCache = memoryCache;
            _mongoGameProvider = mongoGameProvider;
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
            return _mongoGameProvider.CreateGame(game);
        }

        public async Task<GaiaProjectGame> GetGame(string id)
        {
            return await Get(id, () => this._mongoGameProvider.GetGame(id));
        }

        public Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId)
        {
            return this._mongoGameProvider.GetInitialGameState(gameId);
        }

        public Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true)
        {
            return this._mongoGameProvider.GetUserGames(userId, onlyActive);
        }

        public async Task SaveGame(GaiaProjectGame game)
        {
            await this._mongoGameProvider.SaveGame(game);
            this.FlushGame(game.Id);
        }

        public async Task<string> GetPlayerNotes(string playerId, string gameId)
        {
            return await _mongoGameProvider.GetPlayerNotes(playerId, gameId);
        }

        public async Task SavePlayerNotes(string playerId, string gameId, string notes)
        {
            await _mongoGameProvider.SavePlayerNotes(playerId, gameId, notes);
        }
    }
}