using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class TerransConvertPowerFromGaiaDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.TerransDecideIncome;
		public override string Description => "You must decide how to convert the {Power} power returning from Gaia Area";
		public int Power { get; }

		public TerransConvertPowerFromGaiaDecisionViewModel(int power)
		{
			Power = power;
		}
	}
}