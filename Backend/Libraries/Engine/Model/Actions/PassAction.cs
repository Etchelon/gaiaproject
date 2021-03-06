using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(PassAction))]
	public class PassAction : PlayerAction
	{
		public override ActionType Type => ActionType.Pass;
		public RoundBoosterType? SelectedRoundBooster { get; set; }

		public override string ToString()
		{
			return SelectedRoundBooster.HasValue
				? $"passes for the rest of the round and takes booster {SelectedRoundBooster.Value.ToDescription()}"
				: $"passes and finishes its game";
		}
	}
}