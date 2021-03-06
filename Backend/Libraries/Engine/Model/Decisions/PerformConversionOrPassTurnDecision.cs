using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(PerformConversionOrPassTurnDecision))]
	public class PerformConversionOrPassTurnDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.PerformConversionOrPassTurn;

		public override string Description => "can perform conversions before passing";
	}
}
