using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.ActionHandlers.Setup
{
	public class SelectStartingRoundBoosterActionHandler : ActionHandlerBase<SelectStartingRoundBoosterAction>
	{
		private PlayerInGame _player;
		private RoundBoosters _roundBoosters;

		protected override void InitializeImpl(GaiaProjectGame game, SelectStartingRoundBoosterAction action)
		{
			_player = game.Players.First(p => p.Id == action.PlayerId);
			_roundBoosters = game.BoardState.RoundBoosters;
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, SelectStartingRoundBoosterAction action)
		{
			var ret = new List<Effect>
			{
				new AcquireRoundBoosterEffect(action.Booster, null),
				new IncomeVariationEffect(IncomeSource.RoundBooster)
			};
			var players = game.Players.OrderBy(p => p.TurnOrder).ToArray();
			var currentPlayerIndex = _player.TurnOrder - 1;
			if (currentPlayerIndex > 0)
			{
				var nextPlayer = players[currentPlayerIndex - 1];
				ret.Add(new PassTurnToPlayerEffect(nextPlayer.Id, ActionType.SelectStartingRoundBooster));
			}
			else
			{
				ret.Add(new StartRoundsPhaseEffect());
			}
			return ret;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, SelectStartingRoundBoosterAction action)
		{
			if (!IsInGame(action))
			{
				return (false, $"Player cannot take round booster {action.Booster} as it is not available for this game.");
			}
			if (IsTaken(action, out var playerId))
			{
				var owner = game.Players.First(p => p.Id == playerId);
				return (false, $"Player cannot take round booster {action.Booster} as it is already taken by {_player.Username}.");
			}
			return (true, null);
		}

		#region Validation

		private bool IsInGame(SelectStartingRoundBoosterAction action)
		{
			return _roundBoosters.AvailableRoundBooster.Any(rb => rb.Id == action.Booster);
		}

		private bool IsTaken(SelectStartingRoundBoosterAction action, out string playerId)
		{
			var roundBooster = _roundBoosters.AvailableRoundBooster.First(rb => rb.Id == action.Booster);
			playerId = roundBooster.PlayerId;
			return roundBooster.IsTaken;
		}

		#endregion
	}
}
