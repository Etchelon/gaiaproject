using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(PlaceInitialStructureAction))]
	public class PlaceInitialStructureAction : PlayerAction
	{
		public override ActionType Type => ActionType.PlaceInitialStructure;
		public string TargetHexId { get; set; }

		public override string ToString()
		{
			return $"places its initial structure on hex {TargetHexId}";
		}
	}
}