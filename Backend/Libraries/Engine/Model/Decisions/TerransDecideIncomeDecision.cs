using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(TerransDecideIncomeDecision))]
	public class TerransDecideIncomeDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.TerransDecideIncome;
		public int Power { get; set; }
		public override string Description => $"must decide how to convert power returning from Gaia Area";

		public TerransDecideIncomeDecision() { }

		public TerransDecideIncomeDecision(int power)
		{
			Power = power;
		}
	}
}