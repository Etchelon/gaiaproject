using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
    public class SpecialActionSpaceViewModel : IActionSpaceViewModel<Race>
    {
        public string Kind { get; set; }
        public Race Type { get; set; }
        public bool IsAvailable { get; set; }
    }
}