using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Setup;

namespace GaiaProject.Engine.Logic.ActionHandlers.Setup
{
	public class BidForRaceActionHandler : ActionHandlerBase<BidForRaceAction>
	{
		private AuctionState _auctionState;

		protected override void InitializeImpl(GaiaProjectGame game, BidForRaceAction action)
		{
			_auctionState = game.Setup.AuctionState;
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, BidForRaceAction action)
		{
			var bidEffect = new ObtainRaceWithBidEffect(action.Race, action.Points);
			var effects = new List<Effect> { bidEffect };

			var auction = game.Setup.AuctionState;
			var outbidPlayer = auction.Auctions.SingleOrDefault(o => o.Race == action.Race)?.PlayerId;
			var wasLastRace = auction.Auctions.Count == auction.AvailableRaces.Count - 1
				&& auction.Auctions.SingleOrDefault(o => o.Race == action.Race) == null;
			if (wasLastRace)
			{
				effects.Add(new AuctionEndedEffect());
				var firstRaceToPlace = auction.AvailableRaces.First(r => r != Race.Ivits);
				var firstPlayerToPlace = action.Race == firstRaceToPlace
					? action.PlayerId
					: auction.Auctions.Single(o => o.Race == firstRaceToPlace).PlayerId;
				effects.Add(new PassTurnToPlayerEffect(firstPlayerToPlace, ActionType.PlaceInitialStructure));
				effects.Add(new GotoSubphaseEffect(SetupSubPhase.InitialPlacement));
			}
			else
			{
				var sortedPlayers = Game.Players.OrderBy(p => p.TurnOrder).ToList();
				var chainedPlayers = sortedPlayers.Concat(sortedPlayers).ToList();
				var currentPlayerIndex = chainedPlayers.FindIndex(p => p.Id == action.PlayerId);

				var tempAuction = auction.Clone();
				var auctionedRace = tempAuction.Auctions.SingleOrDefault(o => o.Race == action.Race);
				var isNewAuction = auctionedRace == null;
				if (isNewAuction)
				{
					auctionedRace = new AuctionedRace
					{
						Race = action.Race,
						PlayerId = action.PlayerId,
						Bid = action.Points
					};
					tempAuction.Auctions.Add(auctionedRace);
				}
				else
				{
					auctionedRace.PlayerId = action.PlayerId;
					auctionedRace.Bid = action.Points;
				}
				var nextPlayerIndex = chainedPlayers
					.FindIndex(currentPlayerIndex + 1, p => p.Id != action.PlayerId && !tempAuction.Auctions.Any(o => o.PlayerId == p.Id));
				var nextPlayer = chainedPlayers[nextPlayerIndex];
				effects.Add(new PassTurnToPlayerEffect(nextPlayer.Id, ActionType.BidForRace));
			}

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, BidForRaceAction action)
		{
			if (!IsBiddingForExistingRace(action))
			{
				return (false, $"Player is bidding for race {action.Race} which is not in game!!");
			}
			if (IsBiddingForOwnRace(action))
			{
				return (false, $"Player cannot bid for the race he's already controlling.");
			}
			if (IsAlreadyBiddingAnotherRace(action))
			{
				return (false, $"Player cannot bid for the race since he's already controlling another race.");
			}
			if (!HasEnoughPoints(action))
			{
				return (false, $"Player only has {Player.State.Points} and cannot bid more than that.");
			}
			return (true, null);
		}

		#region Validation

		private bool IsBiddingForExistingRace(BidForRaceAction action)
		{
			return _auctionState.AvailableRaces.Any(r => r == action.Race);
		}

		private bool IsBiddingForOwnRace(BidForRaceAction action)
		{
			var auction = _auctionState.Auctions.SingleOrDefault(o => o.Race == action.Race);
			return auction?.PlayerId == action.PlayerId;
		}

		private bool IsAlreadyBiddingAnotherRace(BidForRaceAction action)
		{
			return _auctionState.Auctions.SingleOrDefault(o => o.PlayerId == action.PlayerId) != null;
		}

		private bool HasEnoughPoints(BidForRaceAction action)
		{
			return true; //_player.State.Points >= action.Points;
		}

		#endregion
	}
}
