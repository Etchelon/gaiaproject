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
using MoreLinq;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class QicActionHandler : ActionHandlerBase<QicAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, QicAction action)
		{
			var effects = new List<Effect>
			{
				GetActionCost(action.ActionId),
				new QicActionUsedEffect(action.ActionId)
			};

			PendingDecision actualDecision = new PerformConversionOrPassTurnDecision();
			switch (action.ActionId)
			{
				default:
					throw new ArgumentOutOfRangeException($"Qic action {action.ActionId} not handled");
				case QicActionType.GainTechnologyTile:
					actualDecision = new ChooseTechnologyTileDecision();
					break;
				case QicActionType.RescoreFederationBonus:
					{
						var tokens = Player.State.FederationTokens;
						if (tokens.Count == 0)
						{
							throw new InvalidActionException("You do not have any federation to score");
						}
						if (tokens.Count == 1 || tokens.DistinctBy(t => t.Type).Count() == 1)
						{
							var gain = FederationTokenUtils.GetFederationTokenGain(tokens.First().Type, game, true);
							effects.AddRange(gain.Gains);
						}
						else
						{
							actualDecision = new SelectFederationTokenToScoreDecision(
								tokens.Select(f => f.Type).Distinct().ToList()
							);
						}
						break;
					}
				case QicActionType.GainPointsPerPlanetTypes:
					{
						effects.Add(new PointsGain(3 + Player.State.KnownPlanetTypes.Count, "Qic Action"));
						break;
					}
			}
			effects.Add(new PendingDecisionEffect(actualDecision));
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, QicAction action)
		{
			if (!IsActionAvailable(action.ActionId))
			{
				return (false, "The selected action is not available");
			}
			if (!CanPayCost(action))
			{
				return (false, "You do not have the necessary Qic");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable(QicActionType actionId)
		{
			var researchBoard = Game.BoardState.ResearchBoard;
			return researchBoard.QicActions.Single(pa => pa.Type == actionId).IsAvailable;
		}

		private bool CanPayCost(QicAction action)
		{
			var cost = GetActionCost(action.ActionId);
			return ResourceUtils.CanPayCost(cost, new ActionContext(action, Game), out _);
		}

		#endregion

		private ResourcesCost GetActionCost(QicActionType actionId)
		{
			var rawCost = actionId.GetAttributeOfType<ActionCostAttribute>().Cost;
			return new ResourcesCost(new Resources { Qic = rawCost });
		}
	}
}