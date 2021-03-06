using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class PlaceLostPlanetActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.PlaceLostPlanet;
		public string HexId { get; }

		public PlaceLostPlanetActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}