using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(SortIncomesAction))]
	public class SortIncomesAction : PlayerAction
	{
		public override ActionType Type => ActionType.SortIncomes;
		public List<int> SortedIncomes { get; set; }

		public override string ToString()
		{
			return $"sorts power and power token incomes";
		}
	}
}