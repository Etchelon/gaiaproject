using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class TerransDecideIncomeActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.TerransDecideIncome;
		public int Credits { get; }
		public int Ores { get; }
		public int Knowledge { get; }
		public int Qic { get; }

		public TerransDecideIncomeActionViewModel(int credits, int ores, int knowledge, int qic)
		{
			Credits = credits;
			Ores = ores;
			Knowledge = knowledge;
			Qic = qic;
		}
	}
}