using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class GenericActionViewModel : ActionViewModel
	{
		public override ActionType Type { get; }

		public GenericActionViewModel(ActionType type)
		{
			Type = type;
		}
	}
}