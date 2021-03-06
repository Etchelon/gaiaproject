using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class SelectRoundBoosterActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.SelectStartingRoundBooster;
		public RoundBoosterType Booster { get; set; }

		public SelectRoundBoosterActionViewModel(RoundBoosterType booster)
		{
			Booster = booster;
		}
	}
}
