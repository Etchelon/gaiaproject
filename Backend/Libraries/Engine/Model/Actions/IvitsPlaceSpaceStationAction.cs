using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(IvitsPlaceSpaceStationAction))]
	public class IvitsPlaceSpaceStationAction : PlayerAction
	{
		public override ActionType Type => ActionType.IvitsPlaceSpaceStation;
		public string HexId { get; set; }

		public override string ToString()
		{
			return $"places a space station on hex {HexId}";
		}
	}
}