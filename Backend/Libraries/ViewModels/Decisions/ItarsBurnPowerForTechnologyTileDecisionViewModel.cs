using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class ItarsBurnPowerForTechnologyTileDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.ItarsBurnPowerForTechnologyTile;
		public override string Description =>
			"You must decide whether to burn 4 power from the Gaia Area to get a technology tile (standard or advanced)";
	}
}