using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class PowerManagementUtils
	{
		private const int GainPowerDistance = 2;

		public static List<Effect> GetChargePowerEffects(Hex hex, MapService mapService, PlayerAction action, GaiaProjectGame game)
		{
			var effects = new List<Effect>();

			var isLastRound = game.Rounds.CurrentRound == GaiaProjectGame.LastRound;
			var surroundingBuildings = mapService.GetBuildingsWithinDistance(hex, GainPowerDistance);
			var byPlayer = surroundingBuildings
				.Where(p => p.PlayerId != action.PlayerId)
				.GroupBy(b => b.PlayerId);
			var powersToCharge = byPlayer
				.Select(buildingGroup =>
				{
					var playerId = buildingGroup.Key;
					var player = game.Players.First(p => p.Id == playerId);
					var buildingValue = buildingGroup.Select(b => b.PowerValue).Max();
					var chargeablePower = ChargeablePowerByPlayer(player.State, buildingValue, player.HasPassed);
					// No more charging power decisions after ending the game
					// Free power is charged
					if (isLastRound && player.HasPassed && chargeablePower > 1)
					{
						return (playerId, 0, buildingValue);
					}
					return (playerId, chargeablePower, buildingValue);
				})
				.ToArray();

			foreach (var (playerId, chargeablePower, buildingValue) in powersToCharge)
			{
				var player = game.GetPlayer(playerId);
				var isTaklonsWithPlanetaryInstitute = player.RaceId == Race.Taklons && player.State.Buildings.PlanetaryInstitute;
				if (isTaklonsWithPlanetaryInstitute)
				{
					// In the last round, if the player has passed just charge the token and move it;
					// it could bring the brainstone back to the pool and move it to bowl 2
					if (isLastRound && chargeablePower == 0 && buildingValue == 1)
					{
						var tokenGain = new ResourcesGain(new Resources { PowerTokens = 1 });
						tokenGain.ForPlayer(playerId);
						effects.Add(tokenGain);
						var powerGain = new PowerGain(1);
						powerGain.ForPlayer(playerId);
						effects.Add(powerGain);
						continue;
					}

					var stateAfterToken = player.State.Clone();
					stateAfterToken.Resources.Power.Bowl1 += 1;
					// Since Taklons get a token before charging, always get chargeable power as if they haven't already passed to avoid
					// having the earned token absorbed in the next round's power incomes
					var chargeablePowerAfterToken = ChargeablePowerByPlayer(stateAfterToken, buildingValue, false);

					PendingDecision decision = (chargeablePower != chargeablePowerAfterToken || player.State.Resources.Power.Brainstone == PowerPools.BrainstoneLocation.Removed)
						// Ask Taklons how they want to charge power only when the outcome is different
						? new TaklonsLeechDecision(chargeablePower, chargeablePowerAfterToken) as PendingDecision
						// Otherwise, always ask Taklons if they want to charge power, even if it's just 1 (if they decline, they get no token)
						: ChargePowerDecision.FromBuildOrUpgrade(action, chargeablePower);
					decision.ForPlayer(playerId);
					effects.Add(new PendingDecisionEffect(decision));
					continue;
				}

				if (chargeablePower == 0)
				{
					continue;
				}

				// Itars should always decide whether to charge even 1x0 because they may not be able to burn power to send 4 to Gaia area
				// In the last round it doesn't matter
				if (chargeablePower == 1 && (player.RaceId != Race.Itars || (isLastRound || player.HasPassed)))
				{
					var powerGain = new PowerGain(1);
					powerGain.ForPlayer(playerId);
					effects.Add(powerGain);
				}
				else
				{
					var decision = ChargePowerDecision.FromBuildOrUpgrade(action, chargeablePower);
					decision.ForPlayer(playerId);
					effects.Add(new PendingDecisionEffect(decision));
				}
			}

			return effects;
		}

		private static int ChargeablePowerByPlayer(PlayerState playerState, int buildingPowerValue, bool hasPassed)
		{
			if (playerState.Points == 0)
			{
				return buildingPowerValue == 1 ? 1 : 0;
			}

			var power = playerState.Resources.Power;

			if (hasPassed)
			{
				var needsToChargePower = ChargeablePowerByPlayerAfterIncomes(playerState.Incomes, power.Bowl1, power.Bowl2, power.Brainstone) > 0;
				if (!needsToChargePower)
				{
					return buildingPowerValue == 1 ? 1 : 0;
				}
			}

			var chargeablePower = ChargeablePower(power.Bowl1, power.Bowl2, power.Brainstone);
			var theoretical = Math.Min(buildingPowerValue, chargeablePower);
			return playerState.Points >= theoretical - 1
				? theoretical
				: playerState.Points - 1;
		}

		private static int ChargeablePowerByPlayerAfterIncomes(IEnumerable<Income> incomes, int bowl1, int bowl2, PowerPools.BrainstoneLocation? brainstone = null)
		{
			var totalTokenFromIncomes = incomes.OfType<PowerTokenIncome>().Sum(i => i.PowerTokens);
			var totalPowerFromIncomes = incomes.OfType<PowerIncome>().Sum(i => i.Power);
			var newBowl1 = bowl1 + totalTokenFromIncomes;
			var totalChargeablePower = ChargeablePower(newBowl1, bowl2, brainstone);
			// A player who has passed could charge more than he needs
			return Math.Max(totalChargeablePower - totalPowerFromIncomes, 0);
		}

		private static int ChargeablePower(int bowl1, int bowl2, PowerPools.BrainstoneLocation? brainstone = null)
		{
			var actualBowl1 = bowl1 + (brainstone == PowerPools.BrainstoneLocation.Bowl1 ? 1 : 0);
			var actualBowl2 = bowl2 + (brainstone == PowerPools.BrainstoneLocation.Bowl2 ? 1 : 0);
			return actualBowl1 * 2 + actualBowl2;
		}
	}
}