using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(UpgradeExistingStructureAction))]
	public class UpgradeExistingStructureAction : PlayerAction
	{
		public override ActionType Type => ActionType.UpgradeExistingStructure;
		public string TargetHexId { get; set; }
		public BuildingType TargetBuildingType { get; set; }

		public override string ToString()
		{
			return $"builds a {TargetBuildingType.ToDescription()} on hex {TargetHexId}";
		}
	}
}