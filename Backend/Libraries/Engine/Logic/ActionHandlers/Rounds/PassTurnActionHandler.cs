using System.Collections.Generic;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class PassTurnActionHandler : ActionHandlerBase<PassTurnAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, PassTurnAction action)
		{
			var nextPlayerId = TurnOrderUtils.GetNextPlayer(action.PlayerId, game, true);
			var effects = new List<Effect>
			{
				new PassTurnToPlayerEffect(nextPlayerId)
			};
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, PassTurnAction action)
		{
			if (game.CurrentPlayerId != action.PlayerId)
			{
				return (false, "Player is not the active player.");
			}
			return (true, null);
		}
	}
}
