using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class PlayerPassedEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.CurrentPlayer;
			Debug.Assert(player.Id == PlayerId, "Current player must be the acting one");

			var log = "passes";
			player.Actions.RoundPassed();
			int nextRoundOrder = 0;
			if (game.Rounds.CurrentRound < GaiaProjectGame.LastRound)
			{
				var alreadyPassedPlayers = game.Players.Where(p => p.HasPassed);
				nextRoundOrder = alreadyPassedPlayers.Max(p => p.State.NextRoundTurnOrder ?? 0) + 1;
				player.State.NextRoundTurnOrder = nextRoundOrder;
				log += $" and will play in position {nextRoundOrder} for the next round";
			}
			else
			{
				Loggable = false;
			}
			game.LogEffect(this, log);
		}
	}
}
