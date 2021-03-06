using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class TaklonsLeechDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.TaklonsLeech;
		public override string Description { get; }
		public int PowerBeforeToken { get; }
		public int PowerAfterToken { get; }

		public TaklonsLeechDecisionViewModel(int powerBeforeToken, int powerAfterToken)
		{
			PowerBeforeToken = powerBeforeToken;
			PowerAfterToken = powerAfterToken;
			Description = PowerBeforeToken > 0
				? $"Do you want to charge {PowerBeforeToken} power and then get 1 Token, or get 1 Token and charge {PowerAfterToken}?"
				: $"Do you want to get 1 Token and charge {PowerAfterToken}?";
		}
	}
}