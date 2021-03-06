using System.Linq;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class AuctionEndedEffect : Effect
	{
		public override void ApplyTo(GaiaProjectGame game)
		{
			game.Players.ForEach(p =>
			{
				var auctionedRace = game.Setup.AuctionState.Auctions.Single(o => o.PlayerId == p.Id);
				var raceOrder = game.Setup.AuctionState.AvailableRaces.IndexOf(auctionedRace.Race) + 1;
				p.RaceId = auctionedRace.Race;
				p.State = Factory.InitialPlayerState(auctionedRace.Race, raceOrder, game.Options.StartingVPs, auctionedRace.Bid);
			});
			game.LogSystemMessage("All players have bid for a race.");
		}
	}
}
