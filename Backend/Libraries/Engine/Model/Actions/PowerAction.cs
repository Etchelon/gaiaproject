using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(PowerAction))]
	public class PowerAction : PlayerAction
	{
		public override ActionType Type => ActionType.Power;
		public PowerActionType ActionId { get; set; }

		public override string ToString()
		{
			return $"performs Power action {ActionId.ToDescription()}";
		}
	}
}