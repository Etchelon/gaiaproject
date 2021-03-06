using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class SelectFederationTokenToScoreDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.SelectFederationTokenToScore;
		public override string Description => "You must select a Federation token to score";
	}
}