using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
	public class BoardStateViewModel
	{
		private MapViewModel _map;
		public MapViewModel Map
		{
			get => _map;
			set
			{
				if (_map == value) return;
				_map = value;
			}
		}

		private ScoringBoardViewModel _scoringBoard;
		public ScoringBoardViewModel ScoringBoard
		{
			get => _scoringBoard;
			set
			{
				if (_scoringBoard == value) return;
				_scoringBoard = value;
			}
		}

		private ResearchBoardViewModel _researchBoard;
		public ResearchBoardViewModel ResearchBoard
		{
			get => _researchBoard;
			set
			{
				if (_researchBoard == value) return;
				_researchBoard = value;
			}
		}

		private List<RoundBoosterTileViewModel> _availableRoundBoosters;
		public List<RoundBoosterTileViewModel> AvailableRoundBoosters
		{
			get => _availableRoundBoosters;
			set
			{
				if (_availableRoundBoosters == value) return;
				_availableRoundBoosters = value;
			}
		}

		private List<FederationTokenStackViewModel> _availableFederations;
		public List<FederationTokenStackViewModel> AvailableFederations
		{
			get => _availableFederations;
			set
			{
				if (_availableFederations == value) return;
				_availableFederations = value;
			}
		}
	}
}