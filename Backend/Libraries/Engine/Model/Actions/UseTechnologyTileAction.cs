using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(UseTechnologyTileAction))]
	public class UseTechnologyTileAction : PlayerAction
	{
		public override ActionType Type => ActionType.UseTechnologyTile;
		public int TileId { get; set; }
		public bool Advanced { get; set; }

		public override string ToString()
		{
			if (Advanced)
			{
				var advTile = (AdvancedTechnologyTileType)TileId;
				return $"activates advanced tile ${advTile.ToDescription()}";
			}

			var standardTile = (StandardTechnologyTileType)TileId;
			return $"activates standard tile ${standardTile.ToDescription()}";
		}
	}
}