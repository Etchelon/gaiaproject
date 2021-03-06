using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class FiraksDowngradeResearchLabActionHandler : ActionHandlerBase<FiraksDowngradeResearchLabAction>
	{
		private Hex _targetHex;
		private MapService _mapService;

		protected override void InitializeImpl(GaiaProjectGame game, FiraksDowngradeResearchLabAction action)
		{
			_targetHex = game.BoardState.Map.Hexes.Single(h => h.Id == action.HexId);
			_mapService = new MapService(game.BoardState.Map);
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, FiraksDowngradeResearchLabAction action)
		{
			var effects = new List<Effect>
			{
				new BuildingDeployedEffect(BuildingType.TradingStation, _targetHex.Id, BuildingType.ResearchLab),
				new SpecialActionUsedEffect(null, SpecialActionType.PlanetaryInstitute),
				new IncomeVariationEffect(IncomeSource.Buildings),
				new PendingDecisionEffect(new FreeTechnologyStepDecision())
			};

			var pointGains = PointUtils.GetPointsForBuildingStructure(BuildingType.TradingStation, Player.Id, null, game);
			effects.AddRange(pointGains);

			// Check if other players should gain power
			var chargePowerEffects = PowerManagementUtils.GetChargePowerEffects(_targetHex, _mapService, action, game);
			effects.AddRange(chargePowerEffects);
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, FiraksDowngradeResearchLabAction action)
		{
			if (!IsActionAvailable())
			{
				return (false, "You have already performed a downgrade in this round");
			}
			if (!HasSelectedAResearchLab())
			{
				return (false, "The hex selected for the downgrade should contain a research lab");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable()
		{
			return Player.State.Buildings.PlanetaryInstitute && !Player.Actions.HasUsedPlanetaryInstitute;
		}

		private bool HasSelectedAResearchLab()
		{
			return _targetHex.Buildings.Any(b => b.PlayerId == Player.Id && b.Type == BuildingType.ResearchLab);
		}

		#endregion
	}
}