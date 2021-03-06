using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.ViewModels.Decisions
{
	public class SortIncomesDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.SortIncomes;
		public override string Description => "You must decide in which order to get the power and power token incomes";
		public List<SortableIncomeViewModel> PowerIncomes { get; set; }
		public List<SortableIncomeViewModel> PowerTokenIncomes { get; set; }

		public SortIncomesDecisionViewModel() { }

		public SortIncomesDecisionViewModel(List<PowerIncome> powerIncomes, List<PowerTokenIncome> powerTokenIncomes)
		{
			int i = 0;
			PowerIncomes = powerIncomes.Select(pi => new SortableIncomeViewModel
			{
				Id = i++,
				Type = SortableIncomeType.Power,
				Amount = pi.Power,
				Description = $"{pi.Power} power"
			}).ToList();
			PowerTokenIncomes = powerTokenIncomes.Select(pi => new SortableIncomeViewModel
			{
				Id = i++,
				Type = SortableIncomeType.PowerToken,
				Amount = pi.PowerTokens,
				Description = $"{pi.PowerTokens} tokens"
			}).ToList();
		}
	}
}
