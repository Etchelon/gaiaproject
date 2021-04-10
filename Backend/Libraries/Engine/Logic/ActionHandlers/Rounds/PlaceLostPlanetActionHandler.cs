using System.Collections.Generic;
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
	public class PlaceLostPlanetActionHandler : ActionHandlerBase<PlaceLostPlanetAction>
	{
		private MapService _mapService;
		private Hex _targetHex;
		private bool _isInRange;
		private int _requiredQics = 0;

		protected override void InitializeImpl(GaiaProjectGame game, PlaceLostPlanetAction action)
		{
			_mapService = new MapService(game.BoardState.Map);
			_targetHex = _mapService.GetHex(action.HexId);
			_isInRange = IsInRange();
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, PlaceLostPlanetAction action)
		{
			var effects = new List<Effect>
			{
				new ClearTempStatsEffect(),
				new BuildingDeployedEffect(BuildingType.LostPlanet, action.HexId),
				new HexColonizedEffect(PlanetType.LostPlanet, _targetHex.SectorId),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};
			if (this._requiredQics > 0)
			{
				effects.Insert(0, new ResourcesCost(new Resources { Qic = this._requiredQics }));
			}

			// If Geodens, check if they should earn knowledge
			if (Player.RaceId == Race.Geodens)
			{
				var hasPlanetaryInstitute = Player.State.Buildings.PlanetaryInstitute;
				if (hasPlanetaryInstitute)
				{
					effects.Add(new ResourcesGain(new Resources { Knowledge = 3 }, "Planetary Institute"));
				}
			}

			// Check if player is scoring points for the mine
			var points = PointUtils.GetPointsForBuildingStructure(BuildingType.Mine, Player.Id, PlanetType.LostPlanet, game);
			effects.AddRange(points);

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
							_targetHex.Id,
							clustersAndFederations.Where(o => o.FederationId == null).Select(o => o.Cluster).ToList()
						)
					);
				}
			}

			// Check if other players need to charge power
			var chargePowerEffects = PowerManagementUtils.GetChargePowerEffects(_targetHex, _mapService, action, game);
			effects.AddRange(chargePowerEffects);

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, PlaceLostPlanetAction action)
		{
			if (!IsSpaceHex())
			{
				return (false, "The selected hex is not a space hex");
			}
			if (HasIvitsSpaceStation())
			{
				return (false, "You cannot place the Lost Planet in a hex with an Ivits space station");
			}
			if (!_isInRange)
			{
				return (false, "The target hex is out of range");
			}
			return (true, null);
		}

		#region Validation

		private bool IsSpaceHex()
		{
			return !_targetHex.ActualPlanetType.HasValue;
		}

		private bool HasIvitsSpaceStation()
		{
			return _targetHex.Buildings.SingleOrDefault(b => b.Type == BuildingType.IvitsSpaceStation) != null;
		}

		private bool IsInRange()
		{
			var (hex, requiredQics) = _mapService.GetHexesReachableBy(Player).SingleOrDefault(o => o.hex.Id == _targetHex.Id);
			if (hex == null)
			{
				return false;
			}

			this._requiredQics = requiredQics;
			return Player.State.Resources.Qic >= requiredQics;
		}

		#endregion
	}
}