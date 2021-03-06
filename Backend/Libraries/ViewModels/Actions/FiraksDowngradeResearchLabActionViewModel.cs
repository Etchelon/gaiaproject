using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class FiraksDowngradeResearchLabActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.FiraksDowngradeResearchLab;
		public string HexId { get; }

		public FiraksDowngradeResearchLabActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}