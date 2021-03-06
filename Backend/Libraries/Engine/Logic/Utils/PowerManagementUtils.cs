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

		public static int ChargeablePowerByPlayer(PlayerState playerState, int buildingPowerValue)
		{
			if (playerState.Points == 0)
			{
				return 0;
			}
			var powerPools = playerState.Resources.Power;
			var bowl1 = powerPools.Bowl1 + (powerPools.Brainstone == PowerPools.BrainstoneLocation.Bowl1 ? 1 : 0);
			var bowl2 = powerPools.Bowl2 + (powerPools.Brainstone == PowerPools.BrainstoneLocation.Bowl2 ? 1 : 0);
			var totalChargeablePower = bowl1 * 2 + bowl2;
			var theoretical = Math.Min(buildingPowerValue, totalChargeablePower);
			return playerState.Points >= theoretical - 1
				? theoretical
				: playerState.Points - 1;
		}

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
					var chargeablePower = ChargeablePowerByPlayer(player.State, buildingValue);
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
					var chargeablePowerAfterToken = ChargeablePowerByPlayer(stateAfterToken, buildingValue);

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
				if (chargeablePower == 1 && (player.RaceId != Race.Itars || isLastRound))
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
	}
}