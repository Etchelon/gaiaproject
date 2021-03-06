using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(SelectStartingRoundBoosterAction))]
	public class SelectStartingRoundBoosterAction : PlayerAction
	{
		public override ActionType Type => ActionType.SelectStartingRoundBooster;
		public RoundBoosterType Booster { get; set; }

		public override string ToString()
		{
			return $"will start the game with round booster {Booster.ToDescription()}";
		}
	}
}