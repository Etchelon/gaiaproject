using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(TerransDecideIncomeAction))]
	public class TerransDecideIncomeAction : PlayerAction
	{
		public override ActionType Type => ActionType.TerransDecideIncome;
		public int Credits { get; set; }
		public int Ores { get; set; }
		public int Knowledge { get; set; }
		public int Qic { get; set; }

		public override string ToString()
		{
			return $"converts power from Gaia area";
		}
	}
}