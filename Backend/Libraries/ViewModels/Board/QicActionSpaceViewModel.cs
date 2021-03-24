using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class QicActionSpaceViewModel : IActionSpaceViewModel<QicActionType>
    {
        public string Kind => "qic";
        public QicActionType Type { get; set; }
        public bool IsAvailable { get; set; }
    }
}