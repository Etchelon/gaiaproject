using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class PassTurnToPlayerEffect : Effect
	{
		public List<ActionType> MandatoryActions { get; } = new List<ActionType>();

		public PassTurnToPlayerEffect(string targetPlayer, ActionType? mandatoryAction = null)
		{
			PlayerId = targetPlayer;
			if (mandatoryAction != null)
			{
				MandatoryActions.Add(mandatoryAction.Value);
			}
		}

		public PassTurnToPlayerEffect(string targetPlayer, List<ActionType> mandatoryActions)
		{
			PlayerId = targetPlayer;
			MandatoryActions = mandatoryActions;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var currentPlayer = game.CurrentPlayer;
			// A player's action calls for other actions
			if (currentPlayer != null && currentPlayer.Id == PlayerId && MandatoryActions.Any())
			{
				currentPlayer.Actions.MustTakeAction(MandatoryActions);
				return;
			}

			// When a player passes, the PlayerPassedEffect is executed before this one and
			// therefore there is not a CurrentPlayer anymore
			game.CurrentPlayer?.Actions.TurnPassed();
			var nextPlayer = game.Players.Single(p => p.Id == PlayerId);
			if (MandatoryActions.Any())
			{
				nextPlayer.Actions.MustTakeAction(MandatoryActions);
			}
			else
			{
				nextPlayer.Actions.MustTakeMainAction();
			}
			++game.CurrentTurn;
		}
	}
}
