using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ChargePowerActionHandler : ActionHandlerBase<ChargePowerAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, ChargePowerAction action)
		{
			var effects = new List<Effect>();
			if (!action.Accepted)
			{
				return effects;
			}
			var decision = game.GetPlayer(action.PlayerId).Actions.PendingDecision as ChargePowerDecision;
			var amount = decision.Amount;
			var vp = amount - 1;
			effects.Add(new PowerGain(amount, vp));
			var isTaklonsWithPlanetaryInstitute = Player.RaceId == Race.Taklons && Player.State.Buildings.PlanetaryInstitute;
			if (isTaklonsWithPlanetaryInstitute)
			{
				effects.Add(new ResourcesGain(new Resources { PowerTokens = 1 }));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ChargePowerAction action)
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

			var chargePowerDecision = decision as ChargePowerDecision;
			if (chargePowerDecision == null)
			{
				return (false, "You were not expected to decide whether to charge power");
			}
			return (true, null);
		}
	}
}