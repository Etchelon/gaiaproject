using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class TaklonsLeechActionHandler : ActionHandlerBase<TaklonsLeechAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, TaklonsLeechAction action)
		{
			var effects = new List<Effect>();
			var decision = game.GetPlayer(action.PlayerId).Actions.PendingDecision as TaklonsLeechDecision;
			if (!action.Accepted)
			{
				return effects;
			}

			Debug.Assert(action.ChargeFirstThenToken.HasValue, "action.ChargeFirstThenToken.HasValue");
			var chargeFirst = action.ChargeFirstThenToken.Value;
			if (chargeFirst)
			{
				var amount = decision.ChargeablePowerBeforeToken;
				var vp = amount - 1;
				effects.Add(new PowerGain(amount, vp));
				effects.Add(new ResourcesGain(new Resources { PowerTokens = 1 }));
			}
			else
			{
				effects.Add(new ResourcesGain(new Resources { PowerTokens = 1 }));
				var amount = decision.ChargeablePowerAfterToken;
				var vp = amount - 1;
				effects.Add(new PowerGain(amount, vp));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, TaklonsLeechAction action)
		{
			if (game.PendingDecisions?.FirstOrDefault()?.PlayerId != action.PlayerId)
			{
				return (false, "You are not the active player.");
			}

			var decision = game.GetPlayer(action.PlayerId).Actions.PendingDecision;
			if (decision == null)
			{
				return (false, "You were not expected to take a decision");
			}

			var chargePowerDecision = decision as TaklonsLeechDecision;
			if (chargePowerDecision == null)
			{
				return (false, "You were not expected to decide whether to leech power");
			}
			return (true, null);
		}
	}
}