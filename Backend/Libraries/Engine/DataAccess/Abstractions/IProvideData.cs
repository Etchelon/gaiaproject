using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.DataAccess.Abstractions
{
	public interface IProvideData
	{
		Task<GaiaProjectGame> GetGame(string id);
		Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId);
		Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true);
		Task<string> CreateGame(GaiaProjectGame game);
		Task SaveGame(GaiaProjectGame game);
		Task<User> GetUser(string id);
		Task<User> GetUserByIdentifier(string identifier);
		Task<User> GetUserByUsername(string username);
		Task<string> GetUsername(string userId);
		Task<User[]> GetUsers(Expression<Func<User, bool>> predicate);
		Task<User[]> GetAllUsers();
		Task<string> CreateUser(User user);
		Task UpdateUser(User user);
	}
}