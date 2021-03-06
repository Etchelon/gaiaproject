using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Game
{
	public class GameSubLogViewModel
	{
		public string Timestamp { get; set; }

		public string Message { get; set; }

		public string Player { get; set; }

		public Race? Race { get; set; }
	}
}
