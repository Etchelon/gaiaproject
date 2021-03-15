using GaiaProject.Common.Bus;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Commands
{
	public class CreateGameCommand : ICommand
	{
		public string[] PlayerIds { get; set; }
		public GameOptions Options { get; set; }
	}
}
