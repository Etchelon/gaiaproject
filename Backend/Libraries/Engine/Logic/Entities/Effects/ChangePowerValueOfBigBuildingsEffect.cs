using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class ChangePowerValueOfBigBuildingsEffect : Effect
	{
		public int Variation { get; }

		public ChangePowerValueOfBigBuildingsEffect(int variation = 1)
		{
			Variation = variation;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var mapService = new MapService(game.BoardState.Map);
			var buildingTypes = new List<BuildingType>
			{
				BuildingType.PlanetaryInstitute,
				BuildingType.AcademyLeft,
				BuildingType.AcademyRight
			};
			buildingTypes.ForEach(type =>
			{
				mapService.GetPlayersHexes(PlayerId)
					.WithBuildingType(type)
					.SelectMany(h => h.Buildings)
					// Filter out Lantids parasite buildings
					.Where(b => b.PlayerId == PlayerId)
					.ToList()
					.ForEach(b => b.PowerValue += Variation);
			});
			game.LogEffect(this, $"Planetary Institute and Academies now are worth {(Variation == 1 ? 4 : 3)} power");
		}
	}
}