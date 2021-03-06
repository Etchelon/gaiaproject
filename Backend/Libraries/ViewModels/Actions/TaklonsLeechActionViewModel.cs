using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class TaklonsLeechActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.TaklonsLeech;
		public bool Accepted { get; }
		public bool? ChargeFirstThenToken { get; }

		public TaklonsLeechActionViewModel(bool accepted, bool? chargeFirstThenToken = null)
		{
			Accepted = accepted;
			ChargeFirstThenToken = chargeFirstThenToken;
		}
	}
}