using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ItarsBurnPowerForTechnologyTileActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.ItarsBurnPowerForTechnologyTile;
		public bool Accepted { get; }

		public ItarsBurnPowerForTechnologyTileActionViewModel(bool accepted)
		{
			Accepted = accepted;
		}
	}
}