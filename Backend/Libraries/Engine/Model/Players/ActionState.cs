using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class ActionState
	{
		public bool IsCurrentPlayer { get; set; }
		public ActivationState ActivationState { get; set; } = ActivationState.Inactive;
		public bool CanPerformConversions { get; set; }
		public bool HasPerformedMainAction { get; set; }
		public bool HasUsedPlanetaryInstitute { get; set; }
		public bool HasUsedRightAcademy { get; set; }
		public bool HasUsedRaceAction { get; set; }
		public bool HasPassed { get; set; }
		public List<ActionType> PossibleActions { get; set; } = new List<ActionType>();

		[BsonIgnoreIfNull]
		public PendingDecision PendingDecision { get; set; }

		public ActionState ResetForNewRound()
		{
			IsCurrentPlayer = false;
			ActivationState = ActivationState.Inactive;
			CanPerformConversions = false;
			HasPerformedMainAction = false;
			HasUsedPlanetaryInstitute = false;
			HasUsedRightAcademy = false;
			HasUsedRaceAction = false;
			HasPassed = false;
			return this;
		}

		public void ActionPerformed(ActionType type)
		{
			var isFree = type.HasAttributeOfType<FreeActionAttribute>();
			if (isFree)
			{
				return;
			}

			var isDecision = type.HasAttributeOfType<DecisionAttribute>();
			var maybeDecision = type.HasAttributeOfType<MaybeDecisionAttribute>();
			if (isDecision || maybeDecision && PendingDecision != null)
			{
				PendingDecision = null;
				// When a player is not the current player and he has taken a decision, mark him as inactive
				if (!IsCurrentPlayer)
				{
					ActivationState = ActivationState.Inactive;
					return;
				}
			}

			HasPerformedMainAction = true;
			var hasUsedPlanetaryInstitute = type.HasAttributeOfType<PlanetaryInstituteActionAttribute>();
			if (hasUsedPlanetaryInstitute)
			{
				HasUsedPlanetaryInstitute = true;
			}
			var hasUsedRaceAction = type.HasAttributeOfType<RaceActionAttribute>();
			if (hasUsedRaceAction)
			{
				HasUsedRaceAction = true;
			}
		}

		public void MustTakeAction(ActionType type, bool? canPerformConversions = null)
		{
			PossibleActions = new List<ActionType> { type };
			IsCurrentPlayer = true;
			ActivationState = ActivationState.WaitingForAction;
			var actionPhase = type.GetAttributeOfType<AvailableInPhaseAttribute>()?.Phase ?? throw new Exception($"Enum {type} doesn't have a CurrentPhase attribute");
			CanPerformConversions = (actionPhase == GamePhase.Setup ? null : canPerformConversions) ?? false;
			PendingDecision = null;
		}

		public void MustTakeAction(List<ActionType> possibleActions, bool? canPerformConversions = null)
		{
			PossibleActions = possibleActions;
			IsCurrentPlayer = true;
			ActivationState = ActivationState.WaitingForAction;
			CanPerformConversions = canPerformConversions ?? false;
			PendingDecision = null;
		}

		public void MustTakeMainAction()
		{
			PossibleActions = new List<ActionType>();
			IsCurrentPlayer = true;
			ActivationState = ActivationState.WaitingForAction;
			HasPerformedMainAction = false;
			CanPerformConversions = true;
			PendingDecision = null;
		}

		public void MustTakeDecision(PendingDecision decision)
		{
			ActivationState = ActivationState.WaitingForDecision;
			PendingDecision = decision;
			PossibleActions = new List<ActionType>();
		}

		public void TurnPassed()
		{
			IsCurrentPlayer = false;
			ActivationState = ActivationState.Inactive;
			CanPerformConversions = false;
			PossibleActions = new List<ActionType>();
			PendingDecision = null;
		}

		public void RoundPassed()
		{
			TurnPassed();
			HasPassed = true;
		}

		public static ActionState FromAction(ActionType type, bool? canPerformConversions = null)
		{
			var ret = new ActionState();
			ret.MustTakeAction(type, canPerformConversions);
			return ret;
		}

		public ActionState Clone()
		{
			return new ActionState
			{
				IsCurrentPlayer = IsCurrentPlayer,
				ActivationState = ActivationState,
				CanPerformConversions = CanPerformConversions,
				HasPerformedMainAction = HasPerformedMainAction,
				HasUsedPlanetaryInstitute = HasUsedPlanetaryInstitute,
				HasUsedRightAcademy = HasUsedRightAcademy,
				HasUsedRaceAction = HasUsedRaceAction,
				HasPassed = HasPassed,
				PossibleActions = PossibleActions.ToList(),
				PendingDecision = PendingDecision?.Clone(),
			};
		}
	}
}