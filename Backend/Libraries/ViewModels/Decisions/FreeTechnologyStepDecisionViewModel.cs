using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class FreeTechnologyStepDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.FreeTechnologyStep;
		public override string Description => $"You must decide which technology to research";
	}
}