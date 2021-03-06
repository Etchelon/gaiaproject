using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class PassActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.Pass;
		public RoundBoosterType? SelectedRoundBooster { get; }

		public PassActionViewModel(RoundBoosterType? selectedRoundBooster)
		{
			SelectedRoundBooster = selectedRoundBooster;
		}
	}
}