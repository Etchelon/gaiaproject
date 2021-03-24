using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
    public class ResearchBoardViewModel
    {
        public List<ResearchTrackViewModel> Tracks { get; set; }
        public List<StandardTechnologyTileStackViewModel> FreeStandardTiles { get; set; }
        public List<PowerActionSpaceViewModel> PowerActions { get; set; }
        public List<QicActionSpaceViewModel> QicActions { get; set; }
    }
}