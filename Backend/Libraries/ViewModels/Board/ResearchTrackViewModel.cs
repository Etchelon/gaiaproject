using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class ResearchTrackViewModel
    {
        public ResearchTrackType Id { get; set; }
        public List<PlayerAdvancementViewModel> Players { get; set; }
        public StandardTechnologyTileStackViewModel StandardTiles { get; set; }
        public AdvancedTechnologyTileType? AdvancedTileType { get; set; }
        public FederationTokenType? Federation { get; set; }
        public bool LostPlanet { get; set; }
    }
}