using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class NextTurnEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			++game.CurrentTurn;
		}
	}
}
