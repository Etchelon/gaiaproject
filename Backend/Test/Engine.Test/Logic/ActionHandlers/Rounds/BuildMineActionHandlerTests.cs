using System.Linq;
using System.Threading.Tasks;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Logic.ActionHandlers.Rounds;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Model.Actions;
using Xunit;
using Xunit.Abstractions;

namespace Engine.Test.Logic.ActionHandlers.Rounds
{
	public class BuildMineActionHandlerTests : IClassFixture<TestDataProvider>
	{
		private readonly ITestOutputHelper testOutputHelper;
		private readonly IProvideData dataProvider;
		private readonly ColonizePlanetActionHandler handler = new ColonizePlanetActionHandler();
		private readonly ActionEffectsApplier effectsApplier = new ActionEffectsApplier();

		public BuildMineActionHandlerTests(ITestOutputHelper testOutputHelper, TestDataProvider dataProvider)
		{
			this.testOutputHelper = testOutputHelper;
			this.dataProvider = dataProvider;
		}

		// [Fact]
		public async Task ShouldBuildMineAndNotGivePower()
		{
			var game = await dataProvider.GetGame("ROUNDS_GAME");
			// Test that Ambas build on an adjacent desert planet
			var targetHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "1" && h.PlanetType == PlanetType.Desert);
			var player = game.Players.First(p => p.RaceId == Race.Ambas);
			var playersResources = player.State.Resources;

			// Preconditions
			Assert.Null(targetHex.Buildings.FirstOrDefault());
			Assert.Equal(9, playersResources.Ores);
			Assert.Equal(15, playersResources.Credits);
			Assert.Equal(2, player.State.Buildings.Mines);

			var action = new ColonizePlanetAction
			{
				Id = 1,
				PlayerId = player.Id,
				TargetHexId = targetHex.Id,
			};
			var effects = handler.Handle(game, action);
			var newState = effectsApplier.ApplyActionEffects(action, game, effects);

			// Check that everything is ok
			targetHex = newState.BoardState.Map.Hexes.First(h => h.SectorId == "1" && h.PlanetType == PlanetType.Desert);
			player = newState.Players.First(p => p.RaceId == Race.Ambas);
			playersResources = player.State.Resources;
			var builtMine = targetHex.Buildings.FirstOrDefault();
			Assert.NotNull(builtMine);

			Assert.Equal(Race.Ambas, builtMine.RaceId);
			Assert.Equal(5, playersResources.Ores);
			Assert.Equal(13, playersResources.Credits);
			Assert.Equal(3, player.State.Buildings.Mines);
			var hasSpent4OresForTerraformation = effects.OfType<ResourcesCost>().Any(cost => cost.Resources.Ores == 4);
			Assert.True(hasSpent4OresForTerraformation);

			Assert.Empty(newState.PendingDecisions);
		}

		[Fact]
		public async Task ShouldBeOutOfRange()
		{
			var game = await dataProvider.GetGame("ROUNDS_GAME");
			// Test that Ambas build on an adjacent desert planet
			var targetHex = game.BoardState.Map.Hexes.First(h => h.SectorId == "1" && h.PlanetType == PlanetType.Oxide);
			var player = game.Players.First(p => p.RaceId == Race.Ambas);
			var playersResources = player.State.Resources;

			// Preconditions
			Assert.Null(targetHex.Buildings.FirstOrDefault());

			var action = new ColonizePlanetAction
			{
				Id = 1,
				PlayerId = player.Id,
				TargetHexId = targetHex.Id,
			};
			var exception = Assert.Throws<InvalidActionException>(() => handler.Handle(game, action));
			Assert.Contains("out of range", exception.Message);
		}
	}
}
