using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class SortIncomesActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.SortIncomes;
		public List<int> SortedIncomes { get; }

		public SortIncomesActionViewModel(List<int> sortedIncomes)
		{
			SortedIncomes = sortedIncomes;
		}
	}
}