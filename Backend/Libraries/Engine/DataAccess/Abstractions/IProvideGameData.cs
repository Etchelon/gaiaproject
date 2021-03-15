using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.DataAccess.Abstractions
{
	public interface IProvideGameData
	{
		Task<GaiaProjectGame> GetGame(string id);
		Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId);
		Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true);
		Task<string> CreateGame(GaiaProjectGame game);
		Task SaveGame(GaiaProjectGame game);
	}
}