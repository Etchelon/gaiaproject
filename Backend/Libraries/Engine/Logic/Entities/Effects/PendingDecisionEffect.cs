using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class PendingDecisionEffect : Effect
	{
		public PendingDecision Decision { get; }

		public PendingDecisionEffect(PendingDecision decision)
		{
			Decision = decision;
			ForPlayer(decision.PlayerId);
		}

		public sealed override void ForPlayer(string playerId)
		{
			base.ForPlayer(playerId);
			Decision.ForPlayer(playerId);
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			game.GetPlayer(PlayerId).Actions.MustTakeDecision(Decision);
		}
	}
}
