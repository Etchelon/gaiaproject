using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
	public class SectorViewModel
	{
		private string _id;
		public string Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private int _number;
		public int Number
		{
			get => _number;
			set
			{
				if (_number == value) return;
				_number = value;
			}
		}

		private int _rotation;
		public int Rotation
		{
			get => _rotation;
			set
			{
				if (_rotation == value) return;
				_rotation = value;
			}
		}

		private int _row;
		public int Row
		{
			get => _row;
			set
			{
				if (_row == value) return;
				_row = value;
			}
		}

		private int _column;
		public int Column
		{
			get => _column;
			set
			{
				if (_column == value) return;
				_column = value;
			}
		}

		private List<HexViewModel> _hexes;
		public List<HexViewModel> Hexes
		{
			get => _hexes;
			set
			{
				if (_hexes == value) return;
				_hexes = value;
			}
		}
	}
}