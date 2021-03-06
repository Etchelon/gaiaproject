using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(QicAction))]
	public class QicAction : PlayerAction
	{
		public override ActionType Type => ActionType.Qic;
		public QicActionType ActionId { get; set; }

		public override string ToString()
		{
			return $"performs Qic action {ActionId.ToDescription()}";
		}
	}
}