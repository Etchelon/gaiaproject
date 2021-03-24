using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class PowerActionSpaceViewModel : IActionSpaceViewModel<PowerActionType>
    {
        public string Kind => "power";
        public PowerActionType Type { get; set; }
        public bool IsAvailable { get; set; }
    }
}