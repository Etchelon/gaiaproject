using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class BescodsIncreasePowerValueOfBuildingsOnTitaniumPlanetsEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			var mapService = new MapService(game.BoardState.Map);
			mapService.GetPlayersHexes(PlayerId)
				.OfType(PlanetType.Titanium)
				.SelectMany(h => h.Buildings)
				// Filter out Lantids parasite buildings
				.Where(b => b.PlayerId == PlayerId)
				.ToList()
				.ForEach(b => ++b.PowerValue);
			var player = game.GetPlayer(PlayerId);
			game.LogEffect(this, "buildings on Titanium planets now have a power value increased by 1");
		}
	}
}