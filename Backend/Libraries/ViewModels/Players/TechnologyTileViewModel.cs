using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
    public class TechnologyTileViewModel
    {
        public StandardTechnologyTileType Id { get; set; }
        public AdvancedTechnologyTileType? CoveredByAdvancedTile { get; set; }
        public bool Used { get; set; }
    }
}