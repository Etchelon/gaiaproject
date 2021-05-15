using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class PowerReturnsFromGaiaAreaEffect : Effect
	{
		public int Amount { get; }

		public PowerReturnsFromGaiaAreaEffect(int amount)
		{
			Amount = amount;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			if (Amount == 0)
			{
				return;
			}

			var player = game.GetPlayer(PlayerId);
			var powerPools = player.State.Resources.Power;
			if (powerPools.GaiaArea == 0)
			{
				return;
			}

			var amount = Amount;
			var hasMovedBrainstone = false;
			if (player.RaceId == Race.Taklons && powerPools.Brainstone == PowerPools.BrainstoneLocation.GaiaArea)
			{
				powerPools.Brainstone = PowerPools.BrainstoneLocation.Bowl1;
				amount -= 1;
				hasMovedBrainstone = true;
			}
			powerPools.Bowl1 += amount;
			powerPools.GaiaArea -= amount;
			game.LogPowerReturnsInGaiaPhase(player, amount, hasMovedBrainstone);
		}
	}
}
