using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Setup
{
	public class SelectRaceActionHandler : ActionHandlerBase<SelectRaceAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, SelectRaceAction action)
		{
			var effects = new List<Effect>
			{
				new RaceSelectedEffect(action.Race)
			};

			var sortedPlayers = Game.Players.OrderBy(p => p.TurnOrder).ToList();
			if (game.Options.Auction)
			{
				// Consider that the current player still doesn't have RaceId selected
				var allPlayersHaveSelectedRace = Game.Setup.AuctionState.AvailableRaces.Count + 1 == Game.Players.Count;
				if (allPlayersHaveSelectedRace)
				{
					effects.Add(new LogEffect("All players have picked a race."));
					effects.Add(new PassTurnToPlayerEffect(sortedPlayers.First().Id, ActionType.BidForRace));
					effects.Add(new GotoSubphaseEffect(SetupSubPhase.Auction));
				}
				else
				{
					var nextPlayer = sortedPlayers.Single(p => p.TurnOrder == Player.TurnOrder + 1);
					effects.Add(new PassTurnToPlayerEffect(nextPlayer.Id, ActionType.SelectRace));
				}
			}
			else
			{
				// Consider that the current player still doesn't have RaceId selected
				var allPlayersHaveSelectedRace = Game.Players.Where(p => p.Id != Player.Id).All(p => p.RaceId.HasValue);
				if (allPlayersHaveSelectedRace)
				{
					effects.Add(new LogEffect("All players have picked a race."));
					effects.Add(new PassTurnToPlayerEffect(sortedPlayers.First(p => p.RaceId != Race.Ivits).Id, ActionType.PlaceInitialStructure));
					effects.Add(new GotoSubphaseEffect(SetupSubPhase.InitialPlacement));
				}
				else
				{
					var nextPlayer = sortedPlayers.Single(p => p.TurnOrder == Player.TurnOrder + 1);
					effects.Add(new PassTurnToPlayerEffect(nextPlayer.Id, ActionType.SelectRace));
				}
			}

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, SelectRaceAction action)
		{
			if (HasPickedRaceAlready())
			{
				return (false, "Player has already picked a race.");
			}
			if (RaceHasBeenPickedAlready(game, action))
			{
				return (false, "Another player has already picked this race.");
			}
			return (true, null);
		}

		#region

		private bool HasPickedRaceAlready()
		{
			return Player.RaceId.HasValue;
		}

		private bool RaceHasBeenPickedAlready(GaiaProjectGame game, SelectRaceAction action)
		{
			var pickedRaces = game.Players.Select(p => p.RaceId).Where(race => race.HasValue);
			return pickedRaces.Any(race => race == action.Race);
		}

		#endregion
	}
}
