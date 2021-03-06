using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(PlaceLostPlanetAction))]
	public class PlaceLostPlanetAction : PlayerAction
	{
		public override ActionType Type => ActionType.PlaceLostPlanet;
		public string HexId { get; set; }

		public override string ToString()
		{
			return $"discovers the Lost Planet on hex {HexId}";
		}
	}
}