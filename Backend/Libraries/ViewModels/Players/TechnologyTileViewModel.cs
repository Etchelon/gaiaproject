using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class TechnologyTileViewModel
	{
		private StandardTechnologyTileType _id;
		public StandardTechnologyTileType Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private AdvancedTechnologyTileType? _coveredByAdvancedTile;
		public AdvancedTechnologyTileType? CoveredByAdvancedTile
		{
			get => _coveredByAdvancedTile;
			set
			{
				if (_coveredByAdvancedTile == value) return;
				_coveredByAdvancedTile = value;
			}
		}

		private bool _used;
		public bool Used
		{
			get => _used;
			set
			{
				if (_used == value) return;
				_used = value;
			}
		}
	}
}