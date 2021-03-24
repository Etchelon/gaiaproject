using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
    public class SectorViewModel
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public int Rotation { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public List<HexViewModel> Hexes { get; set; }
    }
}