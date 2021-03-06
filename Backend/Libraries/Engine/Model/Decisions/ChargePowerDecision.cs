using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Actions;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(ChargePowerDecision))]
	public class ChargePowerDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.ChargePower;
		public int Amount { get; set; }

		public override string Description => $"must decide whether to charge {Amount} power for {Amount - 1}VP";

		public static ChargePowerDecision FromBuildOrUpgrade(PlayerAction action, int amount)
		{
			return new ChargePowerDecision
			{
				SpawnedFromActionId = action.Id,
				PlayerId = action.PlayerId,
				Amount = amount
			};
		}
	}
}
