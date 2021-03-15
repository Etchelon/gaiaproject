using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class PowerActionHandler : ActionHandlerBase<PowerAction>
	{
		private int? _powerToBurn;

		protected override List<Effect> HandleImpl(GaiaProjectGame game, PowerAction action)
		{
			var (_, tokensCost) = GetActionCost(action.ActionId);
			var cost = new ResourcesCost(new Resources { Power = tokensCost });
			var effects = new List<Effect>
			{
				cost,
				new PowerActionUsedEffect(action.ActionId)
			};
			if (this._powerToBurn.HasValue)
			{
				var conversionsHandler = new ConversionsActionHandler();
				var dummyConversionAction = new ConversionsAction
				{
					PlayerId = action.PlayerId,
					Conversions = Enumerable.Range(0, this._powerToBurn.Value).Select(__ => Conversion.BurnPower).ToList()
				};
				var conversionEffect = conversionsHandler.Handle(game, dummyConversionAction);
				effects.Insert(0, conversionEffect.Single());
			}

			switch (action.ActionId)
			{
				default:
					throw new ArgumentOutOfRangeException($"Power action {action.ActionId} not handled");
				case PowerActionType.Gain3Knowledge:
				case PowerActionType.Gain2Ores:
				case PowerActionType.Gain7Credits:
				case PowerActionType.Gain2Knowledge:
				case PowerActionType.Gain2PowerTokens:
					{
						var resources = action.ActionId switch
						{
							PowerActionType.Gain3Knowledge => new Resources { Knowledge = 3 },
							PowerActionType.Gain2Ores => new Resources { Ores = 2 },
							PowerActionType.Gain7Credits => new Resources { Credits = 7 },
							PowerActionType.Gain2Knowledge => new Resources { Knowledge = 2 },
							PowerActionType.Gain2PowerTokens => new Resources { PowerTokens = 2 },
							_ => throw new Exception("Impossible")
						};
						effects.Add(new ResourcesGain(resources, "Power Action"));
						break;
					}
				case PowerActionType.Gain2TerraformingSteps:
				case PowerActionType.Gain1TerraformingStep:
					{
						var nSteps = action.ActionId == PowerActionType.Gain2TerraformingSteps ? 2 : 1;
						var gain = new TempTerraformationStepsGain(nSteps);
						effects.Add(gain);
						effects.Add(new PassTurnToPlayerEffect(Player.Id, ActionType.ColonizePlanet));
						return effects;
					}
			}
			effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, PowerAction action)
		{
			if (!IsActionAvailable(action.ActionId))
			{
				return (false, "The selected action is not available");
			}
			if (!CanPayCost(action))
			{
				return (false, "You do not have the necessary power");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable(PowerActionType actionId)
		{
			var researchBoard = Game.BoardState.ResearchBoard;
			return researchBoard.PowerActions.Single(pa => pa.Type == actionId).IsAvailable;
		}

		private bool CanPayCost(PowerAction action)
		{
			var (cost, _) = GetActionCost(action.ActionId);
			var (canPay, powerToBurn) = ResourceUtils.CanPayPowerCost(cost, new ActionContext(action, Game));
			this._powerToBurn = powerToBurn;
			return canPay;
		}

		#endregion

		private (int actualCost, int tokensCost) GetActionCost(PowerActionType actionId)
		{
			var rawCost = actionId.GetAttributeOfType<ActionCostAttribute>().Cost;
			var isNevlasWithPlanetaryInstitute = Player.RaceId == Race.Nevlas && Player.State.Buildings.PlanetaryInstitute;
			if (isNevlasWithPlanetaryInstitute)
			{
				var tokensCost = (int)Math.Ceiling((double)rawCost / 2);
				return (rawCost, tokensCost);
			}
			return (rawCost, rawCost);
		}
	}
}