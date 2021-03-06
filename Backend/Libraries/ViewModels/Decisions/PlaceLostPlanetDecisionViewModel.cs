using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Decisions
{
	public class PlaceLostPlanetDecisionViewModel : PendingDecisionViewModel
	{
		public override PendingDecisionType Type => PendingDecisionType.PlaceLostPlanet;
		public override string Description => "You must place the Lost Planet";
	}
}