using System.Text;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ChooseTechnologyTileAction))]
	public class ChooseTechnologyTileAction : PlayerAction
	{
		public override ActionType Type => ActionType.ChooseTechnologyTile;
		public int TileId { get; set; }
		public bool Advanced { get; set; }
		public StandardTechnologyTileType? CoveredTileId { get; set; }

		public override string ToString()
		{
			if (Advanced)
			{
				var advTile = (AdvancedTechnologyTileType)TileId;
				return $"acquires advanced tile ${advTile.ToDescription()}, covering tile {CoveredTileId!.Value.ToDescription()}";
			}

			var standardTile = (StandardTechnologyTileType)TileId;
			return $"acquires standard tile ${standardTile.ToDescription()}";
		}
	}
}