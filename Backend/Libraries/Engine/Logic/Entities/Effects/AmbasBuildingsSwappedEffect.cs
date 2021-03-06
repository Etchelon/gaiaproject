using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class AmbasBuildingsSwappedEffect : Effect
	{
		public string HexWithPlanetaryInstitute { get; set; }
		public string HexWithMine { get; set; }

		public AmbasBuildingsSwappedEffect(string hexWithPlanetaryInstitute, string hexWithMine)
		{
			HexWithPlanetaryInstitute = hexWithPlanetaryInstitute;
			HexWithMine = hexWithMine;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var planetaryInstitute = game.BoardState.Map.Hexes
				.Single(h => h.Id == HexWithPlanetaryInstitute)
				.Buildings.Single(b => b.Type == BuildingType.PlanetaryInstitute);
			var mine = game.BoardState.Map.Hexes
				.Single(h => h.Id == HexWithMine)
				.Buildings.Single(b => b.PlayerId == PlayerId && b.Type == BuildingType.Mine);

			var planetaryInstitutePowerValue = planetaryInstitute.PowerValue;
			mine.Type = BuildingType.PlanetaryInstitute;
			mine.PowerValue = planetaryInstitute.PowerValue;
			planetaryInstitute.Type = BuildingType.Mine;
			planetaryInstitute.PowerValue = 1;

			// If any of the buildings is in a federation, adjust its power value
			var federations = player.State.Federations;
			var planetaryInstitutesFederation = federations.SingleOrDefault(fed => fed.HexIds.Contains(HexWithPlanetaryInstitute));
			var minesFederation = federations.SingleOrDefault(fed => fed.HexIds.Contains(HexWithMine));
			if (planetaryInstitutesFederation != null)
			{
				planetaryInstitutesFederation.TotalPowerValue -= planetaryInstitutePowerValue - 1;
			}
			if (minesFederation != null)
			{
				minesFederation.TotalPowerValue += planetaryInstitutePowerValue - 1;
			}
		}
	}
}