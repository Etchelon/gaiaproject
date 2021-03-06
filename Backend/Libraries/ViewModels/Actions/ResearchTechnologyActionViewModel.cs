using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ResearchTechnologyActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.ResearchTechnology;
		public ResearchTrackType Track { get; }

		public ResearchTechnologyActionViewModel(ResearchTrackType track)
		{
			Track = track;
		}
	}
}
