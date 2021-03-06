using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class StartGaiaProjectActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.StartGaiaProject;
		public string HexId { get; }

		public StartGaiaProjectActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}