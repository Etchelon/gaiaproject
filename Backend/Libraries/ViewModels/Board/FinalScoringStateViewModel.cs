using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels.Board
{
	public class FinalScoringStateViewModel
	{
		public class PlayerFinalScoringStatusViewModel
		{
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

			private int _count;
			public int Count
			{
				get => _count;
				set
				{
					if (_count == value) return;
					_count = value;
				}
			}

			private int _points;
			public int Points
			{
				get => _points;
				set
				{
					if (_points == value) return;
					_points = value;
				}
			}
		}

		private FinalScoringTileType _tileId;
		public FinalScoringTileType TileId
		{
			get => _tileId;
			set
			{
				if (_tileId == value) return;
				_tileId = value;
			}
		}

		private List<PlayerFinalScoringStatusViewModel> _players;
		public List<PlayerFinalScoringStatusViewModel> Players
		{
			get => _players;
			set
			{
				if (_players == value) return;
				_players = value;
			}
		}
	}
}