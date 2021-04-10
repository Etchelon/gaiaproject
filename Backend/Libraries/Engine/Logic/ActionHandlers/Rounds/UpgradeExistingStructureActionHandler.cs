using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class UpgradeExistingStructureActionHandler : ActionHandlerBase<UpgradeExistingStructureAction>
	{
		private const int GainPowerDistance = 2;
		private ActionContext _ctx;
		private Hex _targetHex;
		private MapService _mapService;

		protected override void InitializeImpl(GaiaProjectGame game, UpgradeExistingStructureAction action)
		{
			_ctx = new ActionContext(action, game);
			_targetHex = Game.BoardState.Map.Hexes.Single(h => h.Id == action.TargetHexId);
			_mapService = new MapService(Game.BoardState.Map);
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, UpgradeExistingStructureAction action)
		{
			var effects = new List<Effect>
			{
				GetActionCost(action.TargetBuildingType),
				new BuildingDeployedEffect(action.TargetBuildingType, _targetHex.Id),
				new IncomeVariationEffect(IncomeSource.Buildings)
			};

			// Check if player is scoring points for the mine
			var points = PointUtils.GetPointsForBuildingStructure(action.TargetBuildingType, Player.Id, _targetHex.PlanetType!.Value, game);
			effects.AddRange(points);

			var buildingsFederation = Player.State.Federations.SingleOrDefault(fed => fed.HexIds.Contains(_targetHex.Id));
			if (buildingsFederation != null && action.TargetBuildingType != BuildingType.ResearchLab)
			{
				var isBigBuilding = action.TargetBuildingType == BuildingType.PlanetaryInstitute
					|| action.TargetBuildingType == BuildingType.AcademyLeft
					|| action.TargetBuildingType == BuildingType.AcademyRight;
				var isBigBuildingWith4Power = isBigBuilding && Player.State.StandardTechnologyTiles.SingleOrDefault(
					t => t.Id == StandardTechnologyTileType.PassiveBigBuildingsWorth4Power && !t.CoveredByAdvancedTile) != null;
				effects.Add(new ChangePowerValueOfFederationEffect(buildingsFederation.Id, isBigBuildingWith4Power ? 2 : 1));
			}
			if (Player.RaceId == Race.Bescods && action.TargetBuildingType == BuildingType.PlanetaryInstitute)
			{
				effects.Add(new BescodsIncreasePowerValueOfBuildingsOnTitaniumPlanetsEffect());
			}
			if (Player.RaceId == Race.Gleens && action.TargetBuildingType == BuildingType.PlanetaryInstitute)
			{
				effects.Add(FederationTokenUtils.GetFederationTokenGain(FederationTokenType.Gleens, game));
			}

			// Check if other players should gain power
			var chargePowerEffects = PowerManagementUtils.GetChargePowerEffects(_targetHex, _mapService, action, game);
			effects.AddRange(chargePowerEffects);

			var shouldTakeTechnologyTile = action.TargetBuildingType == BuildingType.ResearchLab
										   || action.TargetBuildingType == BuildingType.AcademyLeft
										   || action.TargetBuildingType == BuildingType.AcademyRight;

			if (action.AndPass)
			{
				if (shouldTakeTechnologyTile)
				{
					throw new System.Exception("Cannot pass without choosing a technology tile");
				}
				if (action.AndPass)
				{
					var someoneCanChargePower = chargePowerEffects.OfType<PendingDecisionEffect>().Any();
					if (someoneCanChargePower)
					{
						effects.Add(new AutoPassAfterPendingDecisionsEffect());
					}
					else
					{
						var nextPlayerId = TurnOrderUtils.GetNextPlayer(action.PlayerId, game, true);
						effects.Add(new PassTurnToPlayerEffect(nextPlayerId));
					}
				}
				else
				{
					effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
				}
			}
			else
			{
				var decision = shouldTakeTechnologyTile
					? (PendingDecision)new ChooseTechnologyTileDecision()
					: new PerformConversionOrPassTurnDecision();
				effects.Add(new PendingDecisionEffect(decision));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, UpgradeExistingStructureAction action)
		{
			if (!HasBuildingInTargetHex())
			{
				return (false, "You do not have any building to upgrade in the selected hex");
			}
			if (!CanUpgradeSelectedBuilding(action, out var reason))
			{
				return (false, $"You can not upgrade the selected building because {reason}");
			}
			if (!HasSpareBuildings(action.TargetBuildingType))
			{
				return (false, $"You do not have any spare {action.TargetBuildingType} to build");
			}
			return (true, null);
		}

		#region Validation

		private bool HasBuildingInTargetHex()
		{
			var hexBuildings = _targetHex.Buildings;
			return hexBuildings.Any(b => b.PlayerId == Player.Id);
		}

		private bool CanUpgradeSelectedBuilding(UpgradeExistingStructureAction action, out string reason)
		{
			var targetBuildingType = action.TargetBuildingType;
			var sourceBuilding = _targetHex.Buildings.Single(b => b.PlayerId == Player.Id);
			if (Player.RaceId == Race.Lantids && _targetHex.Buildings.Count == 2)
			{
				Debug.Assert(sourceBuilding.Type == BuildingType.Mine, "Lantids cannot have anything else than a mine on a planet with another player");
				reason = "you cannot upgrade a parasite symbiontic mine";
				return false;
			}
			if (!BuildingUtils.CanUpgradeTo(sourceBuilding.Type, targetBuildingType, Player.RaceId.Value))
			{
				reason = $"{sourceBuilding.Type} cannot be upgraded to {targetBuildingType}";
				return false;
			}

			var upgradeCost = GetActionCost(targetBuildingType);
			if (!ResourceUtils.CanPayCost(upgradeCost, _ctx, out reason))
			{
				return false;
			}

			reason = null;
			return true;
		}

		private bool HasSpareBuildings(BuildingType type)
		{
			return BuildingUtils.HasSpareBuildingsOfType(type, Player);
		}

		#endregion

		private ResourcesCost GetActionCost(BuildingType targetBuildingType)
		{
			var isCloseToEnemies = targetBuildingType == BuildingType.TradingStation
				? _mapService.IsInRangeOfOtherPlayers(_targetHex, Player.Id, GainPowerDistance)
				: (bool?)null;
			return ResourcesCost.BuildingCost(targetBuildingType, isCloseToEnemies);
		}
	}
}
