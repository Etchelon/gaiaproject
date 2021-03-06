using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(ItarsBurnPowerForTechnologyTileDecision))]
	public class ItarsBurnPowerForTechnologyTileDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.ItarsBurnPowerForTechnologyTile;
		public override string Description => "must decide whether to burn 4 power tokens from Gaia Area to obtain a technology tile";
	}
}