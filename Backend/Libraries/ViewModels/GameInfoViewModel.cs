using System.Collections.Generic;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.ViewModels
{
    public class GameInfoViewModel : GameBaseViewModel
    {
        public List<PlayerInfoViewModel> Players { get; set; }
    }
}