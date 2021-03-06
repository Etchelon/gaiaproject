using System;
using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class UseRoundBoosterActionHandler : ActionHandlerBase<UseRoundBoosterAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, UseRoundBoosterAction action)
		{
			var booster = Player.State.RoundBooster.Id;
			var effects = new List<Effect>
			{
				new SpecialActionUsedEffect((int)booster, SpecialActionType.RoundBooster)
			};

			Gain gain;
			var possibleActions = new List<ActionType>();
			switch (booster)
			{
				default:
					throw new Exception("Impossible.");
				case RoundBoosterType.BoostRangeGainPower:
					gain = new RangeBoostGain(3);
					possibleActions.AddRange(new[] { ActionType.ColonizePlanet, ActionType.StartGaiaProject });
					break;
				case RoundBoosterType.TerraformActionGainCredits:
					gain = new TempTerraformationStepsGain(1);
					possibleActions.Add(ActionType.ColonizePlanet);
					break;
			}

			effects.Add(gain);
			effects.Add(new PassTurnToPlayerEffect(Player.Id, possibleActions));
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, UseRoundBoosterAction action)
		{
			if (!IsRoundBoosterAvailable())
			{
				return (false, "You have already used your round booster in this round");
			}
			return (true, null);
		}

		#region Validation

		private bool IsRoundBoosterAvailable()
		{
			return !Player.State.RoundBooster.Used;
		}

		#endregion
	}
}