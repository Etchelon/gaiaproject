using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ColonizePlanetActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.ColonizePlanet;
		public string HexId { get; }

		public ColonizePlanetActionViewModel(string hexId)
		{
			HexId = hexId;
		}
	}
}
