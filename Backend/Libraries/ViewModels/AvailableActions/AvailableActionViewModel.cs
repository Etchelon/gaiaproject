using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.AvailableActions
{
	public class AvailableActionViewModel
	{
		public ActionType Type { get; set; }

		public string Description { get; set; }
		public InteractionStateViewModel InteractionState { get; set; }
		public string AdditionalData { get; set; }
	}
}
