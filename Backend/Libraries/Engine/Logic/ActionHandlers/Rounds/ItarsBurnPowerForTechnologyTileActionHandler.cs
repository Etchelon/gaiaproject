using System.Collections.Generic;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ItarsBurnPowerForTechnologyTileActionHandler : ActionHandlerBase<ItarsBurnPowerForTechnologyTileAction>
	{
		private const int PowerTokensToBurn = 4;

		protected override List<Effect> HandleImpl(GaiaProjectGame game, ItarsBurnPowerForTechnologyTileAction action)
		{
			if (!action.Accepted)
			{
				return new List<Effect>
				{
					new PowerReturnsFromGaiaAreaEffect(4)
				};
			}

			return new List<Effect>
			{
				PowerTokensCost.RemoveFromGaiaArea(PowerTokensToBurn),
				new PendingDecisionEffect(new ChooseTechnologyTileDecision())
			};
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ItarsBurnPowerForTechnologyTileAction action)
		{
			if (Player.State.Resources.Power.GaiaArea < PowerTokensToBurn)
			{
				return (false, $"How did you trigger this action? You don't have {PowerTokensToBurn} or more power in the Gaia area!!");
			}
			return (true, null);
		}
	}
}