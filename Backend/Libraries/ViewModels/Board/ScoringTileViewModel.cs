using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class ScoringTileViewModel
	{
		private RoundScoringTileType _tileId;
		public RoundScoringTileType TileId
		{
			get => _tileId;
			set
			{
				if (_tileId == value) return;
				_tileId = value;
			}
		}

		private int _roundNumber;
		public int RoundNumber
		{
			get => _roundNumber;
			set
			{
				if (_roundNumber == value) return;
				_roundNumber = value;
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