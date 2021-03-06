using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class AcceptOrDeclineLastStepActionHandler : ActionHandlerBase<AcceptOrDeclineLastStepAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, AcceptOrDeclineLastStepAction action)
		{
			if (!action.Accepted)
			{
				return new List<Effect>
				{
					new LogEffect($"declines to advance to the last step of track {action.Track.ToDescription()}"),
					new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
				};
			}

			var effects = new List<Effect>();
			effects.AddRange(ResearchUtils.ApplyStep(action.Track, Player.Id, Game, false));
			if (!effects.OfType<PendingDecisionEffect>().Any())
			{
				effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, AcceptOrDeclineLastStepAction action)
		{
			var correspondingDecision = Player.Actions.PendingDecision as AcceptOrDeclineLastStepDecision;
			if (correspondingDecision == null)
			{
				return (false, $"You are not expected to decide whether to advance to the last step of track {action.Track.ToDescription()}");
			}
			if (action.Accepted)
			{
				var playersAdvancement = Player.State.ResearchAdvancements.Single(adv => adv.Track == action.Track);
				if (playersAdvancement.Steps != ResearchUtils.MaxSteps - 1)
				{
					return (false, $"You cannot advance in track {action.Track.ToDescription()} since you are not at level 4");
				}
			}
			return (true, null);
		}
	}
}
