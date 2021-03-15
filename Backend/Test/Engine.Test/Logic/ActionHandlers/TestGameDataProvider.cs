using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Players;
using GaiaProject.Engine.Model.Rounds;

namespace Engine.Test.Logic.ActionHandlers
{
	public class TestGameDataProvider : IProvideGameData
	{
		private readonly List<GaiaProjectGame> _games = new List<GaiaProjectGame>();

		public TestGameDataProvider()
		{
			InitializeData();
		}

		private void InitializeData()
		{
			var usernames = new[] { "Player1", "Player2" };
			var playerIds = usernames.ToArray();
			var boardProvider = new TestBoardProvider(playerIds);
			var races = new List<Race> { Race.Ambas, Race.Terrans };
			var game = new GaiaProjectGame
			{
				Id = "ROUNDS_GAME",
				Name = "Rounds Test Game",
				Players = usernames.Select(username =>
				{
					int index = Array.IndexOf(usernames, username);
					var race = races[index];
					var turnOrder = (index + 1);
					return new PlayerInGame
					{
						Id = username,
						Username = username,
						InitialOrder = turnOrder,
						RaceId = race,
						State = Factory.InitialPlayerState(race, turnOrder)
					};
				})
				.ToList(),
				Created = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
				CreatedBy = "Tester",
				CurrentPhaseId = GamePhase.Rounds,
				CurrentTurn = 1,
				Rounds = new RoundsPhase
				{
					SubPhase = RoundSubPhase.Actions,
					CurrentRound = 1,
				},
				BoardState = boardProvider.Board,
			};

			var ambasPlayer = game.Players[0];
			ambasPlayer.State.Buildings.Mines += 2;
			ambasPlayer.State.RoundBooster = new RoundBooster { Id = RoundBoosterType.GainOreGainKnowledge, Used = false };
			ambasPlayer.State.Resources.Ores += 5;
			ambasPlayer.State.Resources.Knowledge += 2;
			ambasPlayer.Actions.MustTakeMainAction();

			var terranPlayer = game.Players[1];
			terranPlayer.State.Buildings.Mines += 2;
			terranPlayer.State.RoundBooster = new RoundBooster { Id = RoundBoosterType.PassPointsPerBigBuildingsGainPower, Used = false };
			terranPlayer.State.Resources.Ores += 3;
			terranPlayer.State.Resources.Knowledge += 1;
			terranPlayer.State.Resources.Power.Bowl1 -= 4;
			terranPlayer.State.Resources.Power.Bowl2 += 4;
			terranPlayer.State.Gaiaformers[0].Unlocked = true;
			terranPlayer.State.Gaiaformers[0].Available = true;

			var firstAmbasMineHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "1" && h.PlanetType == PlanetType.Swamp);
			var secondAmbasMineHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "7outlined" && h.PlanetType == PlanetType.Swamp);
			firstAmbasMineHex.Buildings = new[] { Building.Factory.Create(BuildingType.Mine, ambasPlayer, firstAmbasMineHex.Id, firstAmbasMineHex.PlanetType!.Value) };
			secondAmbasMineHex.Buildings = new[] { Building.Factory.Create(BuildingType.Mine, ambasPlayer, secondAmbasMineHex.Id, firstAmbasMineHex.PlanetType!.Value) };

			var firstTerranMineHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "1" && h.PlanetType == PlanetType.Terra);
			var secondTerranMineHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "6outlined" && h.PlanetType == PlanetType.Terra);
			firstTerranMineHex.Buildings = new[] { Building.Factory.Create(BuildingType.Mine, terranPlayer, firstTerranMineHex.Id, firstAmbasMineHex.PlanetType!.Value) };
			secondTerranMineHex.Buildings = new[] { Building.Factory.Create(BuildingType.Mine, terranPlayer, secondTerranMineHex.Id, firstAmbasMineHex.PlanetType!.Value) };

			_games.Add(game);
		}

		public async Task<string> CreateGame(GaiaProjectGame game)
		{
			throw new NotImplementedException();
		}

		public async Task<GaiaProjectGame> GetGame(string id)
		{
			var ret = this._games.First(g => g.Id == id);
			return await Task.FromResult(ret);
		}

		public Task<InitialGaiaProjectGameState> GetInitialGameState(string gameId)
		{
			throw new NotImplementedException();
		}

		public Task<GaiaProjectGame[]> GetUserGames(string userId, bool onlyActive = true)
		{
			throw new NotImplementedException();
		}

		public Task SaveGame(GaiaProjectGame game)
		{
			throw new NotImplementedException();
		}
	}
}
