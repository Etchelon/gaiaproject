using System.Collections.Generic;
using System.Linq;
using GaiaProject.ViewModels.Board;
using GaiaProject.ViewModels.Game;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels
{
	public class GameStateViewModel : GameBaseViewModel
	{
		public List<PlayerInGameViewModel> Players { get; set; }
		public BoardStateViewModel BoardState { get; set; }
		public ActivePlayerInfoViewModel ActivePlayer { get; set; }
		public List<GameLogViewModel> GameLogs { get; set; }
		public AuctionStateViewModel AuctionState { get; set; }
		public int CurrentRound => BoardState.ScoringBoard.ScoringTiles
			.FirstOrDefault(st => !st.Inactive)?.RoundNumber ?? 6;
	}
}