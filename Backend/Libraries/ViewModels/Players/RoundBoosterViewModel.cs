using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class RoundBoosterViewModel
	{
		private RoundBoosterType _id;
		public RoundBoosterType Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
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

		private bool _inactive;
		public bool Inactive
		{
			get => _inactive;
			set
			{
				if (_inactive == value) return;
				_inactive = value;
			}
		}
	}
}