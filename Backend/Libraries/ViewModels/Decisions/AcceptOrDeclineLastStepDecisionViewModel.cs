using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class AcceptOrDeclineLastStepDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.AcceptOrDeclineLastStep;
		public override string Description => $"Do you want to advance to the last step of track {Track.ToDescription()} and use one of your federation tokens?";
		public ResearchTrackType Track { get; }

		public AcceptOrDeclineLastStepDecisionViewModel(ResearchTrackType track)
		{
			Track = track;
		}
	}
}