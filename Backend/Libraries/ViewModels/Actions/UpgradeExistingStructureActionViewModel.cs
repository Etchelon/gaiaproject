using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class UpgradeExistingStructureActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.UpgradeExistingStructure;
		public string HexId { get; }
		public BuildingType TargetBuilding { get; }
		public bool AndPass { get; }

		public UpgradeExistingStructureActionViewModel(string hexId, BuildingType targetBuilding, bool andPass)
		{
			HexId = hexId;
			TargetBuilding = targetBuilding;
			// Ignore andPass when building a structure that allows a user to pick a Tech Tile
			AndPass = targetBuilding switch
			{
				BuildingType.ResearchLab => false,
				BuildingType.AcademyLeft => false,
				BuildingType.AcademyRight => false,
				_ => andPass
			};
		}
	}
}
