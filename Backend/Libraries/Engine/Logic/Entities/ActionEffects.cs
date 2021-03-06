using System.Collections.Generic;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Entities
{
	public class ActionEffects
	{
		public PlayerAction Action { get; set; }
		public string Description { get; set; }
		public List<Cost> Costs { get; set; } = new List<Cost>();
		public List<Gain> Gains { get; set; } = new List<Gain>();
		public List<PendingDecision> Decisions { get; set; } = new List<PendingDecision>();

		public ActionEffects(PlayerAction action, string description)
		{
			Action = action;
			Description = description;
		}

		public ActionEffects Add(params Cost[] costs)
		{
			foreach (var cost in costs)
			{
				Costs.Add(cost);
			}
			return this;
		}

		public ActionEffects Add(params Gain[] gains)
		{
			foreach (var gain in gains)
			{
				Gains.Add(gain);
			}
			return this;
		}

		public ActionEffects Add(params PendingDecision[] decisions)
		{
			foreach (var decision in decisions)
			{
				decision.SpawnedFromActionId = Action.Id;
				//gain.LinkToAction(ActionId, Description);
				Decisions.Add(decision);
			}
			return this;
		}
	}
}
