using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class PowerActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.Power;
		public PowerActionType Id { get; }

		public PowerActionViewModel(PowerActionType id)
		{
			Id = id;
		}
	}
}