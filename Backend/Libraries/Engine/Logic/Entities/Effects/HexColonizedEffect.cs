using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class HexColonizedEffect : Effect
	{
		public PlanetType? Type { get; }
		public string Sector { get; }

		public HexColonizedEffect(PlanetType? type, string sector)
		{
			Type = type;
			Sector = sector;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			if (!player.State.ColonizedSectors.Contains(Sector))
			{
				player.State.ColonizedSectors.Add(Sector);
			}
			if (!Type.HasValue)
			{
				return;
			}
			if (Type == PlanetType.Gaia)
			{
				++player.State.GaiaPlanets;
			}
			if (!player.State.KnownPlanetTypes.Contains(Type.Value))
			{
				player.State.KnownPlanetTypes.Add(Type.Value);
			}
		}
	}
}