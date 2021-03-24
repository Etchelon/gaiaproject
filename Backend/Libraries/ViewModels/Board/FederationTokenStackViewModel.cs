using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class FederationTokenStackViewModel
    {
        public FederationTokenType Type { get; set; }
        public int InitialQuantity { get; set; }
        public int Remaining { get; set; }
    }
}