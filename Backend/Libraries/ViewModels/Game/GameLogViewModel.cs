using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Game
{
	public class GameLogViewModel
	{
		public string Timestamp { get; set; }

		public string Message { get; set; }

		public bool Important { get; set; }

		public bool IsSystem { get; set; }

		public int? ActionId { get; set; }

		public int? Turn { get; set; }

		public string Player { get; set; }

		public Race? Race { get; set; }

		public List<GameSubLogViewModel> SubLogs { get; set; }
	}
}
