using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class BidForRaceActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.BidForRace;
		public Race Race { get; }
		public int Points { get; }

		public BidForRaceActionViewModel(Race race, int points)
		{
			Race = race;
			Points = points;
		}
	}
}
