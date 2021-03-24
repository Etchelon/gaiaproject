using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class HexViewModel
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public PlanetType? PlanetType { get; set; }
        public bool WasGaiaformed { get; set; }
        public BuildingViewModel Building { get; set; }
        public BuildingViewModel LantidsParasiteBuilding { get; set; }
        public BuildingViewModel IvitsSpaceStation { get; set; }
        public List<BuildingViewModel> Satellites { get; set; }
    }
}