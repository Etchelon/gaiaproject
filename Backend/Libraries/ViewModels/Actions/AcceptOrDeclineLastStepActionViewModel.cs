using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class AcceptOrDeclineLastStepActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.AcceptOrDeclineLastStep;
		public bool Accepted { get; }
		public ResearchTrackType Track { get; }

		public AcceptOrDeclineLastStepActionViewModel(bool accepted, ResearchTrackType track)
		{
			Accepted = accepted;
			Track = track;
		}
	}
}