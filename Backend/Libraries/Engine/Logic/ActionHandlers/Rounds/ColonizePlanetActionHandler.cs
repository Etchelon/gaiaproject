using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;
using MoreLinq;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ColonizePlanetActionHandler : ActionHandlerBase<ColonizePlanetAction>
	{
		private ActionContext _ctx;
		private Hex _targetHex;
		private MapService _mapService;
		private bool _isGaiaformedTransdim;
		private bool _isLantidsParasiteMine;
		private bool _isInRange;
		private int _requiredQics = 0;

		protected override void InitializeImpl(GaiaProjectGame game, ColonizePlanetAction action)
		{
			_ctx = new ActionContext(action, game);
			_targetHex = game.BoardState.Map.Hexes.Single(h => h.Id == action.TargetHexId);
			_mapService = new MapService(game.BoardState.Map);
			_isGaiaformedTransdim = _targetHex.PlanetType == PlanetType.Transdim && (_targetHex.WasGaiaformed ?? false);
			_isLantidsParasiteMine = Player.RaceId == Race.Lantids &&
									 _targetHex.Buildings.SingleOrDefault(b => b.PlayerId != Player.Id) != null;
			// Calculate requiredQics as a side effect of IsInRange() here.
			// So that when rolling back the game state (without validation) the value is still calculated
			_isInRange = IsInRange();
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, ColonizePlanetAction action)
		{
			var effects = new List<Effect>
			{
				GetActionCost(out var nSteps),
				new IncomeVariationEffect(IncomeSource.Buildings),
				new ClearTempStatsEffect(),
				new BuildingDeployedEffect(BuildingType.Mine, _targetHex.Id)
			};

			// Check if player is scoring points for the mine
			Debug.Assert(_targetHex.ActualPlanetType.HasValue, "_targetHex.PlanetType != null");
			var points = PointUtils.GetPointsForBuildingStructure(BuildingType.Mine, Player.Id, _targetHex.ActualPlanetType.Value, game);
			effects.AddRange(points);
			if (nSteps > 0)
			{
				var scoringTile = game.BoardState.ScoringBoard.ScoringTiles.Single(st => st.RoundNumber == game.Rounds.CurrentRound);
				if (scoringTile.Id == RoundScoringTileType.PointsPerTerraformingStep2)
				{
					effects.Add(new PointsGain(nSteps * 2, "Scoring tile"));
				}
			}

			var isNewPlanetType = !Player.State.KnownPlanetTypes.Contains(_targetHex.ActualPlanetType.Value);
			effects.Add(new HexColonizedEffect(
				_isLantidsParasiteMine ? (PlanetType?)null : _targetHex.ActualPlanetType.Value,
				_targetHex.SectorId
			));

			var hasPlanetaryInstitute = Player.State.Buildings.PlanetaryInstitute;
			// If Geodens, check if they should earn knowledge
			if (Player.RaceId == Race.Geodens && hasPlanetaryInstitute && isNewPlanetType)
			{
				effects.Add(new ResourcesGain(new Resources { Knowledge = 3 }, "Planetary Institute"));
			}

			if (_isLantidsParasiteMine && hasPlanetaryInstitute)
			{
				effects.Add(new ResourcesGain(new Resources { Knowledge = 2 }, "Planetary Institute"));
			}

			// Check if the new mine is enlarging an existing Federation
			var hexesWithPlayerFederations = Player.State.Federations.SelectMany(f => f.HexIds);
			var adjacentHexes = _mapService.GetAdjacentHexes(_targetHex.Id);
			var touchedHexesWithFederation = adjacentHexes
				.Select(h => h.Id)
				.Intersect(hexesWithPlayerFederations)
				.ToList();
			if (touchedHexesWithFederation.Any())
			{
				var clusters = adjacentHexes
					.Select(h => _mapService.GetBuildingCluster(h, Player.Id))
					.NotEmpty()
					.Distinct()
					.ToList();
				var clustersAndFederations = clusters
					.Select(c =>
					{
						var firstHexId = c.First().Id;
						return new
						{
							FederationId = Player.State.Federations.SingleOrDefault(fed => fed.HexIds.Contains(firstHexId))?.Id,
							Cluster = c
						};
					})
					.DistinctBy(o => o.FederationId)
					.ToList();

				var nFederationsToEnlarge = clustersAndFederations.Count(o => o.FederationId != null);
				var nIsolatedClusters = clustersAndFederations.Count - nFederationsToEnlarge;
				if (nFederationsToEnlarge > 1)
				{
					effects.Add(
						new MergeFederationsEffect(
							_targetHex,
							clustersAndFederations.Where(o => o.FederationId != null).Select(o => o.FederationId).ToList(),
							clustersAndFederations.Where(o => o.FederationId == null).Select(o => o.Cluster).ToList()
						)
					);
				}
				else
				{
					var federationToEnlarge = clustersAndFederations.Single(o => o.FederationId != null);
					effects.Add(
						new EnlargeFederationEffect(
							federationToEnlarge.FederationId,
							_targetHex,
							clustersAndFederations.Where(o => o.FederationId == null).Select(o => o.Cluster).ToList()
						)
					);
				}
			}

			// Check if other players should gain power
			var chargePowerEffects = PowerManagementUtils.GetChargePowerEffects(_targetHex, _mapService, action, game);
			effects.AddRange(chargePowerEffects);

			// Player can now decide whether to perform a free conversion or pass the turn
			effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ColonizePlanetAction action)
		{
			if (!IsHexAvailable(out var reason))
			{
				return (false, reason);
			}
			if (!HasMinesAvailable())
			{
				return (false, "You have no mines left to build");
			}
			if (!_isInRange)
			{
				return (false, "The target hex is out of range");
			}
			if (!CanPayCosts(out reason))
			{
				return (false, reason);
			}
			return (true, null);
		}

		#region Validation

		private bool IsHexAvailable(out string reason)
		{
			reason = null;
			if (_isGaiaformedTransdim)
			{
				return true;
			}

			var isOwnedByAnotherPlayer = _targetHex.Buildings.Any(b => b.PlayerId != Player.Id);
			if (isOwnedByAnotherPlayer && !_isLantidsParasiteMine)
			{
				reason = "The target hex is already owned by another player";
				return false;
			}
			var isOwnedByPlayer = _targetHex.Buildings.Any(b => b.PlayerId == Player.Id && b.Type != BuildingType.Gaiaformer);
			if (isOwnedByPlayer)
			{
				reason = "You have already colonized the target hex";
				return false;
			}

			return true;
		}

		private bool HasMinesAvailable()
		{
			return BuildingUtils.HasSpareBuildingsOfType(BuildingType.Mine, Player);
		}

		private bool IsInRange()
		{
			if (_isGaiaformedTransdim)
			{
				return true;
			}
			var (hex, requiredQics) = _mapService.GetHexesReachableBy(Player).SingleOrDefault(o => o.hex.Id == _targetHex.Id);
			if (hex == null)
			{
				return false;
			}

			this._requiredQics = requiredQics;
			return Player.State.Resources.Qic >= requiredQics;
		}

		private bool CanPayCosts(out string reason)
		{
			var rangeBoostCost = new ResourcesCost(new Resources { Qic = this._requiredQics });
			var colonizationCost = _isGaiaformedTransdim || _isLantidsParasiteMine
				? new ResourcesCost(new Resources())
				: TerraformationUtils.GetActualTerraformationCost(Player, _targetHex.PlanetType!.Value, out _);
			var effectiveCost = rangeBoostCost + colonizationCost;
			if (!ResourceUtils.CanPayCost(effectiveCost, _ctx, out reason))
			{
				return false;
			}
			var tempCtx = _ctx.Clone();
			var stateAfterTerraformation = ResourceUtils.ApplyCost(colonizationCost, tempCtx);
			tempCtx.SetPlayerState(stateAfterTerraformation);
			var buildMineCost = ResourcesCost.BuildingCost(BuildingType.Mine);
			if (!ResourceUtils.CanPayCost(buildMineCost, tempCtx, out reason))
			{
				return false;
			}
			return true;
		}

		#endregion

		private ResourcesCost GetActionCost(out int nSteps)
		{
			nSteps = 0;
			var buildingCost = ResourcesCost.BuildingCost(BuildingType.Mine);
			var ret = _isGaiaformedTransdim || _isLantidsParasiteMine
				? buildingCost
				: TerraformationUtils.GetActualTerraformationCost(Player, _targetHex.PlanetType!.Value, out nSteps) + buildingCost;
			if (this._requiredQics > 0)
			{
				ret += new ResourcesCost(new Resources { Qic = this._requiredQics });
			}
			return ret;
		}
	}
}
