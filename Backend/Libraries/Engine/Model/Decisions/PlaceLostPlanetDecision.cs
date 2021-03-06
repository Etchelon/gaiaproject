using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(PlaceLostPlanetDecision))]
	public class PlaceLostPlanetDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.PlaceLostPlanet;
		public string HexId { get; set; }

		public override string Description => $"must decide where to place the Lost Planet.";
	}
}
