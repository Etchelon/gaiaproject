using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class AutoPassAfterPendingDecisionsEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			game.GetPlayer(PlayerId).Actions.AutoPassAfterPendingDecisions = true;
		}
	}
}
