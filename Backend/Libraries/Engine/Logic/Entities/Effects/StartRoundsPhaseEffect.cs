using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class StartRoundsPhaseEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			game.CurrentPhaseId = GamePhase.Rounds;
			game.Rounds = Factory.InitialRoundsPhase(game);
			game.Players.ForEach(p => p.Actions.RoundPassed());
			game.LogSystemMessage("Round Phase Begins", true);
		}
	}
}
