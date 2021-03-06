namespace GaiaProject.ViewModels.Decisions
{
	public class SortableIncomeViewModel
	{
		public int Id { get; set; }
		public SortableIncomeType Type { get; set; }
		public int Amount { get; set; }
		public string Description { get; set; }
	}
}