using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ChargeOrDeclinePowerActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.ChargePower;
		public bool Accepted { get; }

		public ChargeOrDeclinePowerActionViewModel(bool accepted)
		{
			Accepted = accepted;
		}
	}
}