using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
    public class BoardStateViewModel
    {
        public MapViewModel Map { get; set; }
        public ScoringBoardViewModel ScoringBoard { get; set; }
        public ResearchBoardViewModel ResearchBoard { get; set; }
        public List<RoundBoosterTileViewModel> AvailableRoundBoosters { get; set; }
        public List<FederationTokenStackViewModel> AvailableFederations { get; set; }
    }
}