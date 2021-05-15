using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ItarsBurnPowerForTechnologyTileAction))]
	public class ItarsBurnPowerForTechnologyTileAction : PlayerAction
	{
		public override ActionType Type => ActionType.ItarsBurnPowerForTechnologyTile;

		[BsonIgnoreIfDefault]
		public bool Accepted { get; set; }

		public override string ToString()
		{
			return Accepted
				? "takes a technology tile by burning power from Gaia area"
				: "declines to burn power for a technology tile";
		}
	}
}