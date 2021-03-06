using System.Diagnostics;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ChooseTechnologyTileActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.ChooseTechnologyTile;
		public int TileId { get; set; }
		public bool Advanced { get; set; }
		public int? CoveredTileId { get; set; }

		public static ChooseTechnologyTileActionViewModel StandardTile(StandardTechnologyTileType type)
		{
			return new ChooseTechnologyTileActionViewModel
			{
				TileId = (int)type,
				Advanced = false
			};
		}

		public static ChooseTechnologyTileActionViewModel AdvancedTile(AdvancedTechnologyTileType type, StandardTechnologyTileType coveredTile)
		{
			return new ChooseTechnologyTileActionViewModel
			{
				TileId = (int)type,
				Advanced = true,
				CoveredTileId = (int)coveredTile
			};
		}
	}
}