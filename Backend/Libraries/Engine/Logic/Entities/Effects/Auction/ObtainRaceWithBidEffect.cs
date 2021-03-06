using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class ObtainRaceWithBidEffect : Effect
	{
		public Race Race { get; }
		public int Bid { get; }

		public ObtainRaceWithBidEffect(Race race, int bid = 0)
		{
			Race = race;
			Bid = bid;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var auctionedRace = game.Setup.AuctionState.Auctions
				.SingleOrDefault(o => o.Race == Race) ?? new Model.Setup.AuctionedRace { Race = Race };
			var isNewBid = auctionedRace.PlayerId == null;
			auctionedRace.PlayerId = PlayerId;
			auctionedRace.Bid = Bid;
			if (isNewBid)
			{
				game.Setup.AuctionState.Auctions.Add(auctionedRace);
			}
		}
	}
}
