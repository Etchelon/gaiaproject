using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
	public class ScoringBoardViewModel
	{
		private List<ScoringTileViewModel> _scoringTiles;
		public List<ScoringTileViewModel> ScoringTiles
		{
			get => _scoringTiles;
			set
			{
				if (_scoringTiles == value) return;
				_scoringTiles = value;
			}
		}

		private FinalScoringStateViewModel _finalScoring1;
		public FinalScoringStateViewModel FinalScoring1
		{
			get => _finalScoring1;
			set
			{
				if (_finalScoring1 == value) return;
				_finalScoring1 = value;
			}
		}

		private FinalScoringStateViewModel _finalScoring2;
		public FinalScoringStateViewModel FinalScoring2
		{
			get => _finalScoring2;
			set
			{
				if (_finalScoring2 == value) return;
				_finalScoring2 = value;
			}
		}
	}
}