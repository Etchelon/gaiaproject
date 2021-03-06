using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class AmbasSwapPlanetaryInstituteAndMineActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.AmbasSwapPlanetaryInstituteAndMine;
		public string HexId { get; }

		public AmbasSwapPlanetaryInstituteAndMineActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}