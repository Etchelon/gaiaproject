using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.ActionHandlers.Setup
{
	public class PlaceInitialStructureActionHandler : ActionHandlerBase<PlaceInitialStructureAction>
	{
		private ActionContext _ctx;
		private PlayerInGame _player;
		private Hex _targetHex;

		protected override void InitializeImpl(GaiaProjectGame game, PlaceInitialStructureAction action)
		{
			_ctx = new ActionContext(action, game);
			_player = game.Players.First(p => p.Id == action.PlayerId);
			_targetHex = game.BoardState.Map.Hexes.First(h => h.Id == action.TargetHexId);
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, PlaceInitialStructureAction action)
		{
			var ret = new List<Effect>();
			var isIvits = _ctx.Player.RaceId == Race.Ivits;
			ret.Add(new HexColonizedEffect(_targetHex.ActualPlanetType.Value, _targetHex.SectorId));
			ret.Add(new BuildingDeployedEffect(isIvits ? BuildingType.PlanetaryInstitute : BuildingType.Mine, action.TargetHexId));
			ret.Add(new IncomeVariationEffect(IncomeSource.Buildings));

			// Determine next player
			var nextPlayerId = DetermineNextPlayer(game);
			if (nextPlayerId != null)
			{
				ret.Add(new PassTurnToPlayerEffect(nextPlayerId, ActionType.PlaceInitialStructure));
			}
			else
			{
				var lastPlayer = game.Players.OrderBy(p => p.TurnOrder).Last();
				ret.Add(new PassTurnToPlayerEffect(lastPlayer.Id, ActionType.SelectStartingRoundBooster));
				ret.Add(new GotoSubphaseEffect(SetupSubPhase.SelectRoundBoosters));
			}
			return ret;
		}

		private string DetermineNextPlayer(GaiaProjectGame game)
		{
			var playersAndBuildings = game.Players
				.OrderBy(p => p.TurnOrder)
				.Select(p =>
				{
					Debug.Assert(p.RaceId != null, "p.RaceId != null");
					var ret = new
					{
						p.Id,
						Race = p.RaceId.Value,
						BuildingsNo = (p.RaceId == Race.Ivits
							? (p.State.Buildings.PlanetaryInstitute ? 1 : 0)
							: p.State.Buildings.Mines)
							// Effect has not been applied yet so consider that current player has 1 more deployed building
							+ (p.Id == _player.Id ? 1 : 0)
					};
					return ret;
				})
				.ToArray();
			var withoutIvits = playersAndBuildings.Where(pab => pab.Race != Race.Ivits).ToArray();
			// Check if all have placed AT LEAST 2 buildings
			var allHavePlacedTwoBuildings = withoutIvits.All(p => p.BuildingsNo >= 2);
			if (allHavePlacedTwoBuildings)
			{
				var xenosPlayer = playersAndBuildings.FirstOrDefault(p => p.Race == Race.Xenos);
				if (xenosPlayer?.BuildingsNo == 2)
				{
					return xenosPlayer.Id;
				}
				var ivitsPlayer = playersAndBuildings.FirstOrDefault(p => p.Race == Race.Ivits);
				if (ivitsPlayer?.BuildingsNo == 0)
				{
					return ivitsPlayer.Id;
				}
				return null;
			}

			var currentPlayerRace = _player.RaceId.Value;
			var nPlayers = withoutIvits.Length;
			var currentPlayer = withoutIvits.Single(p => p.Race == currentPlayerRace);
			var currentPlayerIndex = Array.IndexOf(withoutIvits, currentPlayer);
			// Check if all have placed AT LEAST 1 building
			var allHavePlacedOneBuilding = withoutIvits.All(p => p.BuildingsNo >= 1);
			if (allHavePlacedOneBuilding)
			{
				var currentPlayerHasPlacedTwoBuildings = currentPlayer.BuildingsNo == 2;
				return currentPlayerHasPlacedTwoBuildings
					? withoutIvits[currentPlayerIndex - 1].Id
					: currentPlayer.Id;
			}
			return withoutIvits[currentPlayerIndex + 1].Id;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, PlaceInitialStructureAction action)
		{
			if (IsSpaceHex())
			{
				return (false, $"Player cannot place a starting mine on an empty space hex.");
			}
			if (!IsNativePlanetType())
			{
				return (false, $"Player cannot place a starting mine on hex of type {_targetHex.PlanetType} because it is not of its native planet type.");
			}
			if (!HexIsEmpty())
			{
				return (false, $"Player cannot place another starting mine on this hex.");
			}
			return (true, null);
		}

		#region Validation

		private bool IsSpaceHex()
		{
			return !_targetHex.PlanetType.HasValue;
		}

		private bool IsNativePlanetType()
		{
			var playersRace = _player.RaceId.Value;
			var racePlanetType = TerraformationUtils.GetRaceNativePlanetType(playersRace);
			return racePlanetType == _targetHex.PlanetType;
		}

		private bool HexIsEmpty()
		{
			return !_targetHex.Buildings.Any();
		}

		#endregion
	}
}
