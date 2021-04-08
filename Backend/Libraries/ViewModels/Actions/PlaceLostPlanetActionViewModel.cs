using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class PlaceLostPlanetActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.PlaceLostPlanet;
		public string HexId { get; }
		public bool AndPass { get; }

		public PlaceLostPlanetActionViewModel(string hexId, bool andPass)
		{
			HexId = hexId;
			AndPass = andPass;
		}
	}
}