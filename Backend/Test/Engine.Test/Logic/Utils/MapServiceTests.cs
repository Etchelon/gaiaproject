using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Engine.Test.Logic.ActionHandlers;
using Engine.Test.Utils;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model.Board;
using MoreLinq.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Engine.Test.Logic.Utils
{

	public class MapServiceTests : IClassFixture<TestGameDataProvider>
	{
		private readonly ITestOutputHelper testOutputHelper;
		private readonly IProvideGameData _gameDataProvider;

		public MapServiceTests(ITestOutputHelper testOutputHelper, TestGameDataProvider gameDataProvider)
		{
			this.testOutputHelper = testOutputHelper;
			this._gameDataProvider = gameDataProvider;
			var converter = new XunitConsoleForwarder(testOutputHelper);
			Console.SetOut(converter);
		}

		[Fact]
		public async Task FindHexesWithinRange3_Test1()
		{
			var game = await _gameDataProvider.GetGame("ROUNDS_GAME");
			var mapService = new MapService(game.BoardState.Map);
			var centralHex = mapService.GetHexAt(5, 12);
			var hexesWithinDistance = mapService.FindHexesWithinDistance(centralHex, 3);
			var expectedHexTypes = new List<PlanetType> { PlanetType.Ice, PlanetType.Gaia, PlanetType.Volcanic };
			var withPlanets = hexesWithinDistance
				.WithConcretePlanet()
				.Select(h => h.PlanetType.Value!)
				.ToList();
			Assert.True(expectedHexTypes.SequenceEqual(withPlanets), "See the map for 2p introductory game");
		}

		[Fact]
		public async Task FindHexesWithinRange3_Test2()
		{
			var game = await _gameDataProvider.GetGame("ROUNDS_GAME");
			var mapService = new MapService(game.BoardState.Map);
			var centralHex = mapService.GetHexAt(3, 12);
			var hexesWithinDistance = mapService.FindHexesWithinDistance(centralHex, 3);
			var expectedHexTypes = new List<PlanetType> { PlanetType.Ice, PlanetType.Gaia, PlanetType.Oxide };
			var withPlanets = hexesWithinDistance
				.WithConcretePlanet()
				.Select(h => h.PlanetType.Value!)
				.ToList();
			Assert.True(expectedHexTypes.SequenceEqual(withPlanets), "See the map for 2p introductory game");
		}

		[Fact]
		public async Task FindHexesInAllMap()
		{
			var game = await _gameDataProvider.GetGame("ROUNDS_GAME");
			var mapService = new MapService(game.BoardState.Map);
			var centralHex = mapService.GetHexAt(12, 7);
			var hexesWithinDistance = mapService.FindHexesWithinDistance(centralHex, 7);
			var withPlanets = hexesWithinDistance.OrderBy(h => h.SectorId).WithConcretePlanet().ToList();
			Assert.True(game.BoardState.Map.Hexes.WithConcretePlanet().Count() == withPlanets.Count(), "With a range of 7, you should reach all hexes from the center of a 2p map!");
		}
	}
}
