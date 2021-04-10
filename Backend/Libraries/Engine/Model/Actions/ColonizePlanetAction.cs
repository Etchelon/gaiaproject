using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ColonizePlanetAction))]
	public class ColonizePlanetAction : PlayerAction
	{
		public override ActionType Type => ActionType.ColonizePlanet;
		public string TargetHexId { get; set; }

		[BsonIgnoreIfDefault]
		public bool AndPass { get; set; }

		public override string ToString()
		{
			return $"builds a mine on hex {TargetHexId}";
		}
	}
}