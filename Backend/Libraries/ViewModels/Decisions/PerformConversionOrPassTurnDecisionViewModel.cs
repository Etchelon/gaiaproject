using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class PerformConversionOrPassTurnDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.PerformConversionOrPassTurn;
		public override string Description => "You can perform conversions before ending your turn";
	}
}