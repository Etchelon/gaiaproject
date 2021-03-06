using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class SelectRaceActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.SelectRace;
		public Race Race { get; }

		public SelectRaceActionViewModel(Race race)
		{
			Race = race;
		}
	}
}
