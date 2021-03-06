using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(TaklonsLeechDecision))]
	public class TaklonsLeechDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.TaklonsLeech;
		public int ChargeablePowerBeforeToken { get; set; }
		public int ChargeablePowerAfterToken { get; set; }
		public override string Description => $"must decide how to charge power";

		public TaklonsLeechDecision() { }

		public TaklonsLeechDecision(int chargeablePowerBeforeToken, int chargeablePowerAfterToken)
		{
			ChargeablePowerBeforeToken = chargeablePowerBeforeToken;
			ChargeablePowerAfterToken = chargeablePowerAfterToken;
		}
	}
}