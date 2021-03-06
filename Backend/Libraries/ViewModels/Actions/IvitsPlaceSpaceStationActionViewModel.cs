using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class IvitsPlaceSpaceStationActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.IvitsPlaceSpaceStation;
		public string HexId { get; }

		public IvitsPlaceSpaceStationActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}