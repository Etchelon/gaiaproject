using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels.Board
{
    public class FinalScoringStateViewModel
    {
        public class PlayerFinalScoringStatusViewModel
        {
            public PlayerInfoViewModel Player { get; set; }
            public int Count { get; set; }
            public int Points { get; set; }
        }

        public FinalScoringTileType TileId { get; set; }
        public List<PlayerFinalScoringStatusViewModel> Players { get; set; }
    }
}