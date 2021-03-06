using System;
using System.Linq;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class TurnOrderUtils
	{
		/// <summary>
		/// Returns the turn order of a player relative to another player.
		/// e.g.: Players A, C, D, B are 1st, 2nd, 3rd and 4th.
		/// Turn order of C relative to B is 2nd, D relative to B is 3rd.
		/// </summary>
		/// <param name="playerId">The player whose turn should be computed</param>
		/// <param name="relativeToPlayerId">The player relative to whom the turn order should be computed.
		/// If null, the first player in turn order will be used.</param>
		/// <param name="game">The game, with all necessary data</param>
		/// <returns></returns>
		public static int GetTurnOrderRelativeTo(string playerId, string relativeToPlayerId, GaiaProjectGame game)
		{
			var nPlayers = game.Players.Count;
			var targetTurnOrder = game.Players.First(p => p.Id == playerId).State.CurrentRoundTurnOrder;
			var pivotPlayerId = relativeToPlayerId ?? game.Players.OrderBy(p => p.TurnOrder).First().Id;
			var pivotTurnOrder = game.Players.First(p => p.Id == pivotPlayerId).State.CurrentRoundTurnOrder;
			return pivotTurnOrder < targetTurnOrder
				? targetTurnOrder - pivotTurnOrder
				: (nPlayers + targetTurnOrder) - pivotTurnOrder;
		}

		/// <summary>
		/// Returns the id of the next player in turn order.
		/// </summary>
		/// <param name="playerId">The current player's id</param>
		/// <param name="game">The game state</param>
		/// <param name="onlyActive">Search only among active players or not</param>
		/// <param name="backwards">If true, goes back from last player to previous from last (used for initial structure placement)</param>
		/// <returns></returns>
		public static string GetNextPlayer(string playerId, GaiaProjectGame game, bool onlyActive, bool backwards = false)
		{
			var nPlayers = game.Players.Count;
			var player = game.Players.Single(p => p.Id == playerId);
			var currentPlayerPosition = player.State.CurrentRoundTurnOrder;
			var playersToConsider = game.Players.Where(p => !onlyActive || !p.HasPassed).OrderBy(p => p.State.CurrentRoundTurnOrder).ToArray();
			if (playersToConsider.Length == 1)
			{
				var activePlayerId = playersToConsider.Single().Id;
				if (playerId != activePlayerId)
				{
					throw new Exception("Active player cannot be different from who performed the action.");
				}
				return activePlayerId;
			}
			var sequence = playersToConsider
				.Concat(backwards ? playersToConsider.Reverse() : playersToConsider)
				.Select(p => p.Id)
				.ToArray();
			var indexOfPlayer = Array.IndexOf(sequence, playerId);
			var nextPlayerId = sequence[indexOfPlayer + 1];
			return nextPlayerId;
		}
	}
}