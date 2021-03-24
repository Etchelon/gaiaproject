using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class MapViewModel
    {
        public int PlayerCount { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public MapShape Shape { get; set; }
        public List<SectorViewModel> Sectors { get; set; }

        public HexViewModel GetHex(string id)
        {
            return Sectors.SelectMany(s => s.Hexes).Single(h => h.Id == id);
        }
    }
}