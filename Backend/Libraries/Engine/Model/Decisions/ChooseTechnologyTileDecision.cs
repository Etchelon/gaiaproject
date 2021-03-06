using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(ChooseTechnologyTileDecision))]
	public class ChooseTechnologyTileDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.ChooseTechnologyTile;
		public override string Description => "must choose a technology tile";
	}
}