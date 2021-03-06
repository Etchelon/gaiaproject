using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(PassTurnAction))]
	public class PassTurnAction : PlayerAction
	{
		public override ActionType Type => ActionType.PassTurn;

		public override string ToString()
		{
			return $"ends its turn";
		}
	}
}