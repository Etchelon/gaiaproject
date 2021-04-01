using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.ActionHandlers.Setup
{
	public class AdjustSectorsActionHandler : ActionHandlerBase<AdjustSectorsAction>
	{
		private MapService _mapService;
		private Map _newMapState;

		protected override void InitializeImpl(GaiaProjectGame game, AdjustSectorsAction action)
		{
			_mapService = new MapService(game.BoardState.Map, true);
			action.Adjustments.ForEach(adj =>
			{
				_mapService.RotateSector(adj.SectorId, adj.Rotation, false);
			});
			_newMapState = _mapService.Map;
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, AdjustSectorsAction action)
		{
			var firstPlayer = game.Players.OrderBy(p => p.TurnOrder).First();
			return new List<Effect>
			{
				new GotoSubphaseEffect(SetupSubPhase.SelectRaces),
				new PassTurnToPlayerEffect(firstPlayer.Id, ActionType.SelectRace),
				new MapAdjustedEffect(_newMapState)
			};
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, AdjustSectorsAction action)
		{
			var (isValid, invalidHexes) = _mapService.FindInvalidHexes();
			if (!isValid)
			{
				return (false, $"The map as you adjusted it is invalid. The following hexes are now adjacent: {string.Join(", ", invalidHexes)}");
			}
			return (true, null);
		}
	}
}
