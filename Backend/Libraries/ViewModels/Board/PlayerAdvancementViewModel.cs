using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class PlayerAdvancementViewModel
	{
		private Race _raceId;
		public Race RaceId
		{
			get => _raceId;
			set
			{
				if (_raceId == value) return;
				_raceId = value;
			}
		}

		private int _steps;
		public int Steps
		{
			get => _steps;
			set
			{
				if (_steps == value) return;
				_steps = value;
			}
		}
	}
}