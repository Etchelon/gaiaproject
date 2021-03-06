using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class UpgradeExistingStructureActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.UpgradeExistingStructure;
		public string HexId { get; }
		public BuildingType TargetBuilding { get; }

		public UpgradeExistingStructureActionViewModel(string hexId, BuildingType targetBuilding)
		{
			HexId = hexId;
			TargetBuilding = targetBuilding;
		}
	}
}
