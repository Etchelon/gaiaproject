using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class IvitsPlaceSpaceStationActionHandler : ActionHandlerBase<IvitsPlaceSpaceStationAction>
	{
		private Hex _targetHex;
		private MapService _mapService;
		private bool _isInRange;
		private int _requiredQics = 0;

		protected override void InitializeImpl(GaiaProjectGame game, IvitsPlaceSpaceStationAction action)
		{
			_targetHex = game.BoardState.Map.Hexes.Single(h => h.Id == action.HexId);
			_mapService = new MapService(game.BoardState.Map);
			// Calculate requiredQics as a side effect of IsInRange() here.
			// So that when rolling back the game state (without validation) the value is still calculated
			_isInRange = IsInRange();
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, IvitsPlaceSpaceStationAction action)
		{
			var effects = new List<Effect>
			{
				new ClearTempStatsEffect(),
				new BuildingDeployedEffect(BuildingType.IvitsSpaceStation, _targetHex.Id),
				new SpecialActionUsedEffect(null, SpecialActionType.PlanetaryInstitute),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};
			if (this._requiredQics > 0)
			{
				effects.Insert(0, new ResourcesCost(new Resources { Qic = this._requiredQics }));
			}

			var ivitsFederation = Player.State.Federations.SingleOrDefault(fed => fed.HexIds.Any());
			if (ivitsFederation == null)
			{
				return effects;
			}

			// Check if the new mine is enlarging an existing Federation
			var hexesInFederation = ivitsFederation.HexIds;
			var adjacentHexes = _mapService.GetAdjacentHexes(_targetHex.Id);
			var touchedHexesWithFederation = adjacentHexes
				.Select(h => h.Id)
				.Intersect(hexesInFederation)
				.ToList();
			if (touchedHexesWithFederation.Any())
			{
				var clusters = adjacentHexes
					.Where(h => !hexesInFederation.Contains(h.Id))
					.Select(h => _mapService.GetBuildingCluster(h, Player.Id))
					.NotEmpty()
					.Distinct()
					.ToList();
				effects.Add(
					new EnlargeFederationEffect(
						ivitsFederation.Id,
						_targetHex.Id,
						clusters
					)
				);
			}

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, IvitsPlaceSpaceStationAction action)
		{
			if (!IsActionAvailable())
			{
				return (false, "You have already placed a Space Station in this round");
			}
			if (!IsHexAvailable(out var reason))
			{
				return (false, reason);
			}
			if (!_isInRange)
			{
				return (false, "The selected hex is not in range");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable()
		{
			return !Player.Actions.HasUsedPlanetaryInstitute;
		}

		private bool IsHexAvailable(out string reason)
		{
			var hasPlanet = _targetHex.ActualPlanetType != null;
			if (hasPlanet)
			{
				reason = "The target hex is actually a planet, not a space area";
				return false;
			}

			var isOccupiedByLostPlanet = _targetHex.ActualPlanetType == PlanetType.LostPlanet;
			if (isOccupiedByLostPlanet)
			{
				reason = "The target hex is occupied by the Lost Planet";
				return false;
			}
			reason = null;
			return true;
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