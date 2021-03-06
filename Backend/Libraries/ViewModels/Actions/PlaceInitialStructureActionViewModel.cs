using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class PlaceInitialStructureActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.PlaceInitialStructure;
		public string HexId { get; set; }

		public PlaceInitialStructureActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}
