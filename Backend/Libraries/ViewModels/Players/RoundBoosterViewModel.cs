using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
    public class RoundBoosterViewModel
    {
        public RoundBoosterType Id { get; set; }
        public bool Used { get; set; }
        public bool Inactive { get; set; }
    }
}