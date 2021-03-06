using System.Diagnostics;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class UseTechnologyTileActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.UseTechnologyTile;
		public int TileId { get; set; }
		public bool Advanced { get; set; }

		// Do not remove, it's used for JSON deserialization to/from server.
		// Also leave the setters in the properties
		public UseTechnologyTileActionViewModel() { }

		public UseTechnologyTileActionViewModel(StandardTechnologyTileType? standardTile, AdvancedTechnologyTileType? advancedTile)
		{
			Debug.Assert(standardTile.HasValue || advancedTile.HasValue, "Pass either a standard tile or an advanced tile");
			Advanced = advancedTile.HasValue;
			TileId = Advanced ? (int)advancedTile.Value : (int)standardTile.Value;
		}
	}
}