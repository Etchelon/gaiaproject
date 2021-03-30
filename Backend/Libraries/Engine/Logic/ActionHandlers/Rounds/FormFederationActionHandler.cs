using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;
using MoreLinq;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class FormFederationActionHandler : ActionHandlerBase<FormFederationAction>
	{
		private MapService _mapService;
		private List<Hex> _hexesWithBuildings;
		private List<Hex> _hexesWithSatellites;
		private List<Hex> _federatedHexes;

		protected override void InitializeImpl(GaiaProjectGame game, FormFederationAction action)
		{
			_mapService = new MapService(game.BoardState.Map);
			_hexesWithBuildings =
				game.BoardState.Map.Hexes.Where(h => action.SelectedBuildings.Contains(h.Id)).ToList();
			_hexesWithSatellites =
				game.BoardState.Map.Hexes.Where(h => action.SelectedSatellites.Contains(h.Id)).ToList();
			_federatedHexes = _hexesWithBuildings.Concat(_hexesWithSatellites)
				.SelectMany(h => _mapService.GetBuildingCluster(h, Player.Id))
				.Concat(_hexesWithSatellites)
				.DistinctBy(h => h.Id)
				.ToList();
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, FormFederationAction action)
		{
			var effects = new List<Effect>
			{
				FederationTokenUtils.GetFederationTokenGain(action.SelectedFederationToken, game),
				new FederationTokenTakenEffect(action.SelectedFederationToken),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};

			var cost = GetActionCost();
			if (cost != null)
			{
				effects.Add(cost);
			}

			//// Add buildings in federation
			//var federatedHexes = _hexesWithBuildings.Concat(_hexesWithSatellites).ToList();

			//// Find all other adjacent buildings that were not selected (because the selected ones already have enough power)
			//var additionalFederatedHexes = _mapService.GetPlayersHexes(Player.Id, false, Player.RaceId == Race.Lantids)
			//	// Exclude the selected ones for this federation
			//	.Where(h => !federatedHexes.Any(fh => fh.Id == h.Id))
			//	// Exclude those already in a Federation
			//	.Where(h => !Player.State.Federations.Any(fed => fed.HexIds.Contains(h.Id)))
			//	// Adjacent to hexes used for the newly formed federation
			//	.Where(h =>
			//	{
			//		var adjacentHexIds = _mapService.GetAdjacentHexes(h.Id).Select(adjH => adjH.Id);
			//		return federatedHexes.Select(fedH => fedH.Id).Intersect(adjacentHexIds).Any();
			//	})
			//	.ToList();
			//federatedHexes = federatedHexes.Concat(additionalFederatedHexes).ToList();

			effects.Add(Player.RaceId == Race.Ivits && Player.State.Federations.Count == 1
				? (Effect)new IvitsFederationExpandedEffect(_federatedHexes)
				: new FederationCreatedEffect(_federatedHexes)
			);

			// Place satellites
			effects.AddRange(action.SelectedSatellites.Select(hexId => new BuildingDeployedEffect(BuildingType.Satellite, hexId)));

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, FormFederationAction action)
		{
			if (SelectedBuildingsAreInAnotherFederation())
			{
				return (false, "Some of the selected buildings are already part of another federation.");
			}
			if (!CanPayForSatellites(out var reason))
			{
				return (false, reason);
			}
			if (!IsSelectedTokenAvailable(action.SelectedFederationToken))
			{
				return (false, "The are no remaining tokens of the selected type");
			}
			if (!HasEnoughPower())
			{
				return (false, "The federation doesn't have enough power");
			}

			// The following checks are to be reworked so currently they all return false
			if (HasHoles())
			{
				return (false, "Some of the buildings are not connected");
			}
			if (HasNeedlessBuildings())
			{
				return (false, "You must create the federation using fewer buildings");
			}
			if (HasNeedlessSatellites())
			{
				return (false, "You must create the federation using fewer satellites");
			}
			return (true, null);
		}

		#region Validation

		private bool SelectedBuildingsAreInAnotherFederation()
		{
			if (Player.RaceId == Race.Ivits)
			{
				return false;
			}

			return Player.State.Federations.Any(fed =>
				fed.HexIds.Intersect(_hexesWithBuildings.Select(h => h.Id)).Any()
			);
		}

		private bool CanPayForSatellites(out string reason)
		{
			var cost = GetActionCost();
			if (cost == null)
			{
				reason = null;
				return true;
			}
			return ResourceUtils.CanPayCost(cost, new ActionContext(new NullAction { PlayerId = Player.Id }, Game), out reason);
		}

		private bool HasHoles()
		{
			return false;
		}

		private bool HasEnoughPower()
		{
			var powerRequired = GetRequiredPower();
			return _hexesWithBuildings
				.Select(h => h.Buildings.First(b => b.PlayerId == Player.Id))
				.Select(b => b.PowerValueInFederation)
				.Sum() >= powerRequired;
		}

		private bool HasNeedlessBuildings()
		{
			return false;

			// TODO: rework validation logic in a dedicared branch
			var powerRequired = GetRequiredPower();

			var clustersFromBuildings = _hexesWithBuildings
				.Select(h => _mapService.GetBuildingCluster(h, Player.Id))
				.Distinct()
				.ToList();
			var clustersPowerValues = clustersFromBuildings
				.Select(hexes => hexes.Select(h => h.Buildings.Single(b => b.PlayerId == Player.Id).PowerValueInFederation).Sum())
				.ToList();

			for (var i = 0; i < clustersPowerValues.Count; ++i)
			{
				var others = clustersPowerValues.ToList();
				others.RemoveAt(i);
				var sumOfOthers = others.Sum();
				if (sumOfOthers >= powerRequired)
				{
					return true;
				}
			}

			return false;
		}

		// TODO
		private bool HasNeedlessSatellites()
		{
			return false;
		}

		private bool IsSelectedTokenAvailable(FederationTokenType type)
		{
			return Game.BoardState.Federations.Tokens.Single(t => t.Type == type).Remaining > 0;
		}

		#endregion

		private int GetRequiredPower()
		{
			if (Player.RaceId == Race.Ivits && (Player.State.Federations?.Any() ?? false))
			{
				var ivitsFederation = Player.State.Federations.Single();
				var (canTakeMoreTokens, excessPowerValueOfFederation) = FederationTokenUtils.CanIvitsTakeMoreTokens(Player, ivitsFederation);
				return canTakeMoreTokens ? 0 : (7 - excessPowerValueOfFederation);
			}

			return Player.RaceId == Race.Xenos && Player.State.Buildings.PlanetaryInstitute
				? 6
				: 7;
		}

		private Cost GetActionCost()
		{
			var satellitesCount = _hexesWithSatellites.Count;
			if (satellitesCount == 0)
			{
				return null;
			}

			return Player.RaceId == Race.Ivits
				? (Cost)new ResourcesCost(new Resources { Qic = satellitesCount })
				: PowerTokensCost.Remove(satellitesCount, Player);
		}
	}
}