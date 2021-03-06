using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class LogEffect : Effect
	{
		public string Message { get; }

		public LogEffect(string message)
		{
			Message = message;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			if (!string.IsNullOrEmpty(PlayerId))
			{
				var player = game.GetPlayer(PlayerId);
				game.LogEffect(this, Message);
			}
			else
			{
				game.LogSystemMessage(Message);
			}
		}
	}
}