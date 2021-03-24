using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class ScoringTileViewModel
    {
        public RoundScoringTileType TileId { get; set; }
        public int RoundNumber { get; set; }
        public bool Inactive { get; set; }
    }
}