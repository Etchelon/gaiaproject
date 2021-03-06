using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class ChooseTechnologyTileDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.ChooseTechnologyTile;
		public override string Description => "You must decide which technology tile to take";
	}
}