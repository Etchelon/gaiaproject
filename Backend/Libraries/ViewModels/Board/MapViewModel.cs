using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class MapViewModel
	{
		private int _playerCount;
		public int PlayerCount
		{
			get => _playerCount;
			set
			{
				if (_playerCount == value) return;
				_playerCount = value;
			}
		}

		private int _rows;
		public int Rows
		{
			get => _rows;
			set
			{
				if (_rows == value) return;
				_rows = value;
			}
		}

		private int _columns;
		public int Columns
		{
			get => _columns;
			set
			{
				if (_columns == value) return;
				_columns = value;
			}
		}

		private MapShape _shape;
		public MapShape Shape
		{
			get => _shape;
			set
			{
				if (_shape == value) return;
				_shape = value;
			}
		}

		private List<SectorViewModel> _sectors;
		public List<SectorViewModel> Sectors
		{
			get => _sectors;
			set
			{
				if (_sectors == value) return;
				_sectors = value;
			}
		}

		public HexViewModel GetHex(string id)
		{
			return Sectors.SelectMany(s => s.Hexes).Single(h => h.Id == id);
		}
	}
}