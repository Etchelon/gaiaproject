using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(TaklonsLeechAction))]
	public class TaklonsLeechAction : PlayerAction
	{
		public override ActionType Type => ActionType.TaklonsLeech;
		public bool Accepted { get; set; }
		public bool? ChargeFirstThenToken { get; set; }

		public override string ToString()
		{
			return Accepted
				? ((ChargeFirstThenToken ?? false)
				? "charges power and gains a new power token"
				: "gains a new power token and charges power")
				: "declines to charge power";
		}
	}
}