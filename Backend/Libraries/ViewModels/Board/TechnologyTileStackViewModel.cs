using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class StandardTechnologyTileStackViewModel
	{
		private StandardTechnologyTileType _type;
		public StandardTechnologyTileType Type
		{
			get => _type;
			set
			{
				if (_type == value) return;
				_type = value;
			}
		}

		private int _total;
		public int Total
		{
			get => _total;
			set
			{
				if (_total == value) return;
				_total = value;
			}
		}

		private int _remaining;
		public int Remaining
		{
			get => _remaining;
			set
			{
				if (_remaining == value) return;
				_remaining = value;
			}
		}
	}
}