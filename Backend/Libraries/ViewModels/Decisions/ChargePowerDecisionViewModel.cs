using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class ChargePowerDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.ChargePower;
		public override string Description => $"Do you want to charge {Power} power for {Points} points?";
		public int Power { get; }
		public int Points => Power - 1;

		public ChargePowerDecisionViewModel(int power)
		{
			Power = power;
		}
	}
}