using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels.Board
{
    public class RoundBoosterTileViewModel
    {
        public RoundBoosterType Id { get; set; }
        public bool IsTaken { get; set; }
        public PlayerInfoViewModel Player { get; set; }
        public bool Used { get; set; }
    }
}