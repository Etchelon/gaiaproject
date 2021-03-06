using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(UseRightAcademyAction))]
	public class UseRightAcademyAction : PlayerAction
	{
		public override ActionType Type => ActionType.UseRightAcademy;

		public override string ToString()
		{
			return $"activates the Academy";
		}
	}
}