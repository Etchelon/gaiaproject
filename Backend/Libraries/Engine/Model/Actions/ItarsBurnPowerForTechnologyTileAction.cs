using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ItarsBurnPowerForTechnologyTileAction))]
	public class ItarsBurnPowerForTechnologyTileAction : PlayerAction
	{
		public override ActionType Type => ActionType.ItarsBurnPowerForTechnologyTile;

		public override string ToString()
		{
			return $"takes a technology tile by burning power from Gaia area";
		}
	}
}