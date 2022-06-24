using System;
using System.Threading.Tasks;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.DataAccess.Abstractions
{
	public interface IProvideGameData
	{
		Task<GaiaProjectGame> GetGame(string id);
		Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId);
		Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true);
		Task<GaiaProjectGame[]> GetAllGames(bool active, int skip, int take);
		Task<long> CountAllGames(bool active);
		Task<string> CreateGame(GaiaProjectGame game);
		Task SaveGame(GaiaProjectGame game);
		Task<string> GetPlayerNotes(string playerId, string gameId);
		Task SavePlayerNotes(string playerId, string gameId, string notes);
		Task DeleteGame(string id);
	}
}