using System.Collections.Generic;
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
	public class UseRightAcademyActionHandler : ActionHandlerBase<UseRightAcademyAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, UseRightAcademyAction action)
		{
			var isBalTaks = Player.RaceId == Race.BalTaks;
			var resources = new Resources { Credits = isBalTaks ? 4 : 0, Qic = isBalTaks ? 0 : 1 };
			return new List<Effect>
			{
				new SpecialActionUsedEffect(null, SpecialActionType.RightAcademy),
				new ResourcesGain(resources),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, UseRightAcademyAction action)
		{
			if (!IsActionAvailable())
			{
				return (false, "You have already used the academy in this round");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable()
		{
			return Player.State.Buildings.AcademyRight && !Player.Actions.HasUsedRightAcademy;
		}

		#endregion
	}
}