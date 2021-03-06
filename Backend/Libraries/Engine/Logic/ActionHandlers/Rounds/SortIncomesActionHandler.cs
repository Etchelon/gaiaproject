using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class SortIncomesActionHandler : ActionHandlerBase<SortIncomesAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, SortIncomesAction action)
		{
			var decision = Player.Actions.PendingDecision as SortIncomesDecision;
			Debug.Assert(decision != null, nameof(decision) + " != null");
			var incomesWithIds = new Dictionary<int, Income>();
			int i = 0;
			decision.PowerIncomes.ForEach(pi => incomesWithIds.Add(i++, pi));
			decision.PowerTokenIncomes.ForEach(pti => incomesWithIds.Add(i++, pti));
			return action.SortedIncomes.Select<int, Effect>(incomeId =>
			{
				var income = incomesWithIds[incomeId];
				return income switch
				{
					PowerIncome pi => new PowerGain(pi.Power),
					PowerTokenIncome pti => new ResourcesGain(new Resources { PowerTokens = pti.PowerTokens }),
					_ => throw new ArgumentOutOfRangeException("Income type")
				};
			})
			.ToList();
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, SortIncomesAction action)
		{
			var decision = Player.Actions.PendingDecision as SortIncomesDecision;
			if (decision == null)
			{
				return (false, "You are not expected to decide how to sort incomes");
			}

			var numIncomes = decision.PowerIncomes.Count + decision.PowerTokenIncomes.Count;
			if (numIncomes != action.SortedIncomes.Count)
			{
				return (false, "Mismatch between server data and data sent by the user");
			}
			return (true, null);
		}
	}
}