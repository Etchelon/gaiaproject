using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels.Board
{
	public class RoundBoosterTileViewModel
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

		private bool _isTaken;
		public bool IsTaken
		{
			get => _isTaken;
			set
			{
				if (_isTaken == value) return;
				_isTaken = value;
			}
		}

		private PlayerInfoViewModel _player;
		public PlayerInfoViewModel Player
		{
			get => _player;
			set
			{
				if (_player == value) return;
				_player = value;
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