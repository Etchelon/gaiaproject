using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class AmbasSwapPlanetaryInstituteAndMineActionHandler : ActionHandlerBase<AmbasSwapPlanetaryInstituteAndMineAction>
	{
		private Hex _targetHex;
		private MapService _mapService;

		protected override void InitializeImpl(GaiaProjectGame game, AmbasSwapPlanetaryInstituteAndMineAction action)
		{
			_targetHex = game.BoardState.Map.Hexes.Single(h => h.Id == action.HexId);
			_mapService = new MapService(game.BoardState.Map);
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, AmbasSwapPlanetaryInstituteAndMineAction action)
		{
			var hexWithPlanetaryInstitute = _mapService.GetPlayersHexes(Player.Id).Single(
				h => h.Buildings.Any(b => b.Type == BuildingType.PlanetaryInstitute)
			);
			var effects = new List<Effect>
			{
				new AmbasBuildingsSwappedEffect(hexWithPlanetaryInstitute.Id, _targetHex.Id),
				new SpecialActionUsedEffect(null, SpecialActionType.PlanetaryInstitute),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, AmbasSwapPlanetaryInstituteAndMineAction action)
		{
			if (!IsActionAvailable())
			{
				return (false, "You have already performed the swap in this round");
			}
			if (!HasSelectedAMine())
			{
				return (false, "The hex selected for the swap should contain a mine");
			}
			return (true, null);
		}

		#region Validation

		private bool IsActionAvailable()
		{
			return Player.State.Buildings.PlanetaryInstitute && !Player.Actions.HasUsedPlanetaryInstitute;
		}

		private bool HasSelectedAMine()
		{
			return _targetHex.Buildings.Any(b => b.PlayerId == Player.Id && b.Type == BuildingType.Mine);
		}

		#endregion
	}
}