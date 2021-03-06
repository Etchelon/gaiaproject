using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class RaceSelectedEffect : Effect
	{
		public Race Race { get; }

		public RaceSelectedEffect(Race race)
		{
			Race = race;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			if (game.Options.Auction)
			{
				game.Setup.AuctionState.AvailableRaces.Add(Race);
			}
			else
			{
				var player = game.GetPlayer(PlayerId);
				player.RaceId = Race;
				player.State = Factory.InitialPlayerState(Race, player.InitialOrder, game.Options.StartingVPs);
			}
		}
	}
}