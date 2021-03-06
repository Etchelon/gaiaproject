using System.Collections.Generic;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class TerransDecideIncomeActionHandler : ActionHandlerBase<TerransDecideIncomeAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, TerransDecideIncomeAction action)
		{
			return new List<Effect>
			{
				new ResourcesGain(GetResources(action), "Terran's Planetary Institute")
			};
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, TerransDecideIncomeAction action)
		{
			if (!Player.State.Buildings.PlanetaryInstitute)
			{
				return (false, "You have not built the Planetary Institute");
			}
			if (WantsTooMuch(action))
			{
				return (false, "You do not have enough power in the Gaia area for all these conversions");
			}
			return (true, null);
		}

		#region Validation

		private bool WantsTooMuch(TerransDecideIncomeAction action)
		{
			var resources = GetResources(action);
			var powerRequired = resources.Credits + resources.Ores * 3 + (resources.Knowledge + resources.Qic) * 4;
			// Inexact check as there could have been more than 0 tokens in Bowl 2 before the return of
			// the tokens from the Gaia Area
			return powerRequired > Player.State.Resources.Power.Bowl2;
		}

		#endregion

		private Resources GetResources(TerransDecideIncomeAction action) => new Resources
		{
			Credits = action.Credits,
			Ores = action.Ores,
			Knowledge = action.Knowledge,
			Qic = action.Qic
		};
	}
}