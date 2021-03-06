using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	[BsonKnownTypes(typeof(ResourceIncome), typeof(PowerIncome), typeof(PowerTokenIncome))]
	[BsonDiscriminator(RootClass = true)]
	public abstract class Income
	{
		public IncomeSource SourceType { get; set; }
		public string SourceId { get; set; }
		public abstract IncomeType Type { get; }

		public abstract Income Clone();
	}

	[BsonDiscriminator(nameof(ResourceIncome))]
	public class ResourceIncome : Income
	{
		public override IncomeType Type => IncomeType.Resources;
		public int Credits { get; set; }
		public int Ores { get; set; }
		public int Knowledge { get; set; }
		public int Qic { get; set; }

		public override Income Clone()
		{
			return new ResourceIncome
			{
				SourceType = SourceType,
				SourceId = SourceId,
				Credits = Credits,
				Ores = Ores,
				Knowledge = Knowledge,
				Qic = Qic
			};
		}
	}

	[BsonDiscriminator(nameof(PowerIncome))]
	public class PowerIncome : Income
	{
		public override IncomeType Type => IncomeType.Power;
		public int Power { get; set; }

		public override Income Clone()
		{
			return new PowerIncome
			{
				SourceType = SourceType,
				SourceId = SourceId,
				Power = Power
			};
		}
	}

	[BsonDiscriminator(nameof(PowerTokenIncome))]
	public class PowerTokenIncome : Income
	{
		public override IncomeType Type => IncomeType.PowerToken;
		public int PowerTokens { get; set; }

		public override Income Clone()
		{
			return new PowerTokenIncome
			{
				SourceType = SourceType,
				SourceId = SourceId,
				PowerTokens = PowerTokens
			};
		}
	}
}