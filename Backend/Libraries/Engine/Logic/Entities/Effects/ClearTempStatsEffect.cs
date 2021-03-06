using System.Linq;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class ClearTempStatsEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			player.ClearTempEffects();
		}
	}
}