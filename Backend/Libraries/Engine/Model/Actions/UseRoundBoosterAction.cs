using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(UseRoundBoosterAction))]
	public class UseRoundBoosterAction : PlayerAction
	{
		public override ActionType Type => ActionType.UseRoundBooster;

		public override string ToString()
		{
			return $"activates the round booster";
		}
	}
}