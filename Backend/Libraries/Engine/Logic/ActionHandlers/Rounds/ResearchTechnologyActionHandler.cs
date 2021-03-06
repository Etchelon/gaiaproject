using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ResearchTechnologyActionHandler : ActionHandlerBase<ResearchTechnologyAction>
	{
		private int _targetLevel;

		protected override void InitializeImpl(GaiaProjectGame game, ResearchTechnologyAction action)
		{
			var playerAdvancements = Player.State.ResearchAdvancements.Single(adv => adv.Track == action.TrackId);
			_targetLevel = playerAdvancements.Steps + 1;
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, ResearchTechnologyAction action)
		{
			var actionCost = GetActionCost();
			var effects = new List<Effect>();
			if (actionCost != null)
			{
				effects.Add(actionCost);
			}
			effects.AddRange(ResearchUtils.ApplyStep(action.TrackId, Player.Id, Game, true));
			if (!effects.OfType<PendingDecisionEffect>().Any())
			{
				effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ResearchTechnologyAction action)
		{
			if (!CanAdvanceInTrack(action.TrackId, out var reason))
			{
				return (false, $"You cannot advance any further in track {action.TrackId} because {reason}");
			}
			if (!CanPayCost(action))
			{
				return (false, "You do not have the necessary resources to research a technology");
			}
			return (true, null);
		}

		#region Validation

		private bool CanAdvanceInTrack(ResearchTrackType track, out string reason)
		{
			var (can, why) = ResearchUtils.CanPlayerAdvanceToLevel(_targetLevel, track, Player.Id, Game);
			reason = why;
			return can;
		}

		private bool CanPayCost(ResearchTechnologyAction action)
		{
			var cost = GetActionCost();
			return cost == null || ResourceUtils.CanPayCost(cost, new ActionContext(action, Game), out _);
		}

		#endregion

		private ResourcesCost GetActionCost()
		{
			var isFreeStep = Player.Actions.PendingDecision?.Type == PendingDecisionType.FreeTechnologyStep;
			if (isFreeStep)
			{
				return null;
			}
			return ResourcesCost.ResearchAdvancementCost(Player.RaceId!.Value);
		}
	}
}
