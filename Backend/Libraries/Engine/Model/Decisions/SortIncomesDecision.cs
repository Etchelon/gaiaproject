using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Players;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(SortIncomesDecision))]
	public class SortIncomesDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.SortIncomes;
		public List<PowerIncome> PowerIncomes { get; set; }
		public List<PowerTokenIncome> PowerTokenIncomes { get; set; }
		public override string Description => "must decide how to sort power incomes";

		public static SortIncomesDecision FromIncomes(List<PowerIncome> powerGains, List<PowerTokenIncome> powerTokenGains)
		{
			return new SortIncomesDecision
			{
				PowerIncomes = powerGains,
				PowerTokenIncomes = powerTokenGains
			};
		}
	}
}