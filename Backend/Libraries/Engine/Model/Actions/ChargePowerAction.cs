using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ChargePowerAction))]
	public class ChargePowerAction : PlayerAction
	{
		public override ActionType Type => ActionType.ChargePower;
		public bool Accepted { get; set; }

		public override string ToString()
		{
			return Accepted ? "charges power" : "declines to charge power";
		}
	}
}