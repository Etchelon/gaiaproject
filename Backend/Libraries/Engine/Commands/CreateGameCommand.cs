using GaiaProject.Engine.Model;
using ScoreSheets.Common.Bus;

namespace GaiaProject.Engine.Commands
{
	public class CreateGameCommand : ICommand
	{
		public string[] PlayerIds { get; set; }
		public GameOptions Options { get; set; }
	}
}
