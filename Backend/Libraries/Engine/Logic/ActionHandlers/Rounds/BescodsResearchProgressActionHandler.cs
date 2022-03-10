using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class BescodsResearchProgressActionHandler : ActionHandlerBase<BescodsResearchProgressAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, BescodsResearchProgressAction action)
		{
			var researchableTechnologies = MoreLinq.MoreEnumerable.MinBy(Player.State.ResearchAdvancements, adv => adv.Steps)
				.Select(adv => adv.Track)
				.ToList();
			return new List<Effect>
			{
				new SpecialActionUsedEffect(null, SpecialActionType.RaceAction),
				new PendingDecisionEffect(new FreeTechnologyStepDecision(researchableTechnologies))
			};
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, BescodsResearchProgressAction action)
		{
			if (!IsActionAvailable())
			{
				return (false, "You have already performed the swap in this round");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable()
		{
			return !Player.Actions.HasUsedRaceAction;
		}

		#endregion
	}
}