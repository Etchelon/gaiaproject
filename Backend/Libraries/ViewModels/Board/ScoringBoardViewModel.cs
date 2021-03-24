using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
    public class ScoringBoardViewModel
    {
        public List<ScoringTileViewModel> ScoringTiles { get; set; }
        public FinalScoringStateViewModel FinalScoring1 { get; set; }
        public FinalScoringStateViewModel FinalScoring2 { get; set; }
    }
}