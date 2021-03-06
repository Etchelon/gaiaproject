using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class ResourceUtils
	{
		const int MaxCredits = 30;
		const int MaxOres = 15;
		const int MaxKnowledge = 15;
		const int BrainstonePowerValue = 3;

		/// <summary>
		/// Returns the starting resources for the specified race
		/// </summary>
		/// <param name="race">The id of the race</param>
		internal static PlayerResources GetBaseResources(Race race)
		{
			return new PlayerResources
			{
				Credits = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialCredits),
				Ores = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialOres),
				Knowledge = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialKnowledge),
				Qic = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialQic),
				Power = new PowerPools
				{
					Bowl1 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl1),
					Bowl2 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl2),
					Bowl3 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl3),
					GaiaArea = 0
				}
			};
		}

		#region Costs

		/// <summary>
		/// Checks whether a player can pay a certain cost
		/// </summary>
		/// <param name="cost">The cost to pay</param>
		/// <param name="ctx">The action context</param>
		/// <param name="reason">The reason why the cost can't be paid, if so</param>
		public static bool CanPayCost(Cost cost, ActionContext ctx, out string reason)
		{
			reason = null;
			return cost switch
			{
				ResourcesCost rc => CanPayCost(rc, ctx, out reason),
				PowerTokensCost rc => CanPayCost(rc, ctx, out reason),
				StartGaiaProjectCost gpc => CanPayCost(gpc, ctx, out reason),
				UseFederationTokenCost ufc => CanPayCost(cost, ctx, out reason),
				_ => throw new Exception($"Cost of type {cost.Type} not handled.")
			};
		}

		/// <summary>
		/// Checks whether a player can pay the specified resources
		/// </summary>
		/// <param name="cost">The cost to pay</param>
		/// <param name="ctx">The action context</param>
		/// <param name="reason">The reason why the cost can't be paid, if so</param>
		private static bool CanPayCost(ResourcesCost cost, ActionContext ctx, out string reason)
		{
			var player = ctx.Player;
			var playersResources = ctx.PlayerState.Resources;
			var can = true;
			var reasons = new StringBuilder("");
			if (cost.Credits > 0 && playersResources.Credits < cost.Credits)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Credits} credits.");
			}
			if (cost.Ores > 0 && playersResources.Ores < cost.Ores)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Ores} ores.");
			}
			if (cost.Knowledge > 0 && playersResources.Knowledge < cost.Knowledge)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Knowledge} knowledge.");
			}
			if (cost.Qic > 0 && playersResources.Qic < cost.Qic)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Qic} qic.");
			}
			if (cost.Power > 0)
			{
				var isNevlasWithPlanetaryInstitute = player.RaceId.Value == Race.Nevlas && player.State.Buildings.PlanetaryInstitute;
				var requiredPower = isNevlasWithPlanetaryInstitute
					? (int)Math.Ceiling((double)cost.Power / 2)
					: cost.Power;
				int availablePower = playersResources.Power.Bowl3;

				var hasBrainstoneInBowl3 = player.RaceId == Race.Taklons && playersResources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl3;
				if (hasBrainstoneInBowl3)
				{
					availablePower += BrainstonePowerValue;
				}

				if (availablePower < requiredPower)
				{
					can = false;
					reasons.AppendLine($"You do not have the required {requiredPower} power in bowl 3.");
				}
			}
			reason = reasons.ToString();
			return can;
		}

		public static (bool can, int? toBurn) CanPayPowerCost(int power, ActionContext ctx)
		{
			Debug.Assert(power >= 0, "A Power cost cannot be negative");
			if (power == 0)
			{
				return (true, null);
			}

			var player = ctx.Player;
			var playersResources = ctx.PlayerState.Resources;
			var isNevlasWithPlanetaryInstitute = player.RaceId.Value == Race.Nevlas && player.State.Buildings.PlanetaryInstitute;
			var requiredPower = isNevlasWithPlanetaryInstitute
				? (int)Math.Ceiling((double)power / 2)
				: power;
			int availablePower = playersResources.Power.Bowl3;
			int potentialPowerFromBurning = (int)Math.Floor((double)playersResources.Power.Bowl2 / 2);

			var hasBrainstoneInBowl2 = player.RaceId == Race.Taklons && playersResources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl2;
			if (hasBrainstoneInBowl2)
			{
				// If normal power in bowl2 is even, one of those must remain in bowl2 to have brainstone moved to bowl3
				// If it is odd, the odd token is burned to move the brainstone and other power already accounted for stays the same
				potentialPowerFromBurning += BrainstonePowerValue - 1 + (playersResources.Power.Bowl2 % 2);
			}

			var hasBrainstoneInBowl3 = player.RaceId == Race.Taklons && playersResources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl3;
			if (hasBrainstoneInBowl3)
			{
				availablePower += BrainstonePowerValue;
			}

			if (availablePower < requiredPower)
			{
				var difference = requiredPower - availablePower;
				var canBurn = difference <= potentialPowerFromBurning;
				if (canBurn)
				{
					var toBurn = hasBrainstoneInBowl2
						? difference <= BrainstonePowerValue
							? 1
							// e.g.: 0/5(B)/0 -> can get 5 power by burning 3: 0/0/2(B)
							// if normal power in bowl 2 is even, potential power is 1 less
							// 0/4(B)/0 -> can get 4 power by burning 2: 0/1/1(B)
							: potentialPowerFromBurning / 2 + potentialPowerFromBurning % 2
						: difference;
					return (true, toBurn);
				}
				return (false, null);
			}
			return (true, null);
		}

		/// <summary>
		/// Checks whether a player can remove the specified power tokens from its bowls
		/// </summary>
		/// <param name="cost">The cost to pay</param>
		/// <param name="ctx">The action context</param>
		/// <param name="reason">The reason why the cost can't be paid, if so</param>
		private static bool CanPayCost(PowerTokensCost cost, ActionContext ctx, out string reason)
		{
			var can = true;
			var reasons = new StringBuilder("");
			var power = ctx.PlayerState.Resources.Power;
			if (power.Bowl1 < cost.Bowl1)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Bowl1} power tokens in power bowl 1.");
			}
			if (power.Bowl2 < cost.Bowl2)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Bowl2} power tokens in power bowl 2.");
			}
			if (power.Bowl3 < cost.Bowl3)
			{
				can = false;
				reasons.AppendLine($"You do not have the required {cost.Bowl3} power tokens in power bowl 3.");
			}
			if (cost.Brainstone
				&& (power.Brainstone == PowerPools.BrainstoneLocation.GaiaArea || power.Brainstone == PowerPools.BrainstoneLocation.Removed))
			{
				can = false;
				reasons.AppendLine($"Player's brainstone is not in any power bowl.");
			}
			reason = reasons.ToString();
			return can;
		}

		private static bool CanPayCost(StartGaiaProjectCost _, ActionContext ctx, out string reason)
		{
			var playerState = ctx.Player.State;
			var hasAnAvailableGaiaformer = playerState.Gaiaformers.Any(gf => gf.Unlocked && gf.Available);
			if (!hasAnAvailableGaiaformer)
			{
				reason = "You do not have an available Gaiaformer to use.";
				return false;
			}

			reason = null;
			return true;
		}

		private static bool CanPayCost(UseFederationTokenCost tokenCost, ActionContext ctx, out string reason)
		{
			reason = null;
			var state = ctx.PlayerState;
			var hasAFederationToUse = state.FederationTokens.Any(fed => !fed.UsedForTechOrAdvancedTile);
			if (!hasAFederationToUse)
			{
				reason = "You do not have a spare federation to use";
			}
			return hasAFederationToUse;
		}

		/// <summary>
		/// Applies a cost to a player's state. Returns the new player's state.
		/// </summary>
		/// <param name="cost">The cost to apply</param>
		/// <param name="ctx">The action context</param>
		internal static PlayerState ApplyCost(Cost cost, ActionContext ctx)
		{
			return cost switch
			{
				ResourcesCost rc => ApplyCost(rc, ctx),
				PowerTokensCost ptc => ApplyCost(ptc, ctx),
				UseFederationTokenCost ufc => ApplyCost(ufc, ctx),
				_ => throw new Exception($"Cost of type {cost.Type} not handled.")
			};
		}

		private static PlayerState ApplyCost(ResourcesCost cost, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var playersRace = ctx.Player.RaceId!.Value;
			var playersResources = state.Resources;
			playersResources.Credits -= cost.Credits;
			playersResources.Ores -= cost.Ores;
			playersResources.Knowledge -= cost.Knowledge;
			playersResources.Qic -= cost.Qic;

			var powerToSpend = cost.Power;
			var power = playersResources.Power;
			var hasBrainstoneInBowl3 = playersRace == Race.Taklons && power.Brainstone == PowerPools.BrainstoneLocation.Bowl3;
			if (hasBrainstoneInBowl3 && powerToSpend >= BrainstonePowerValue)
			{
				power.Brainstone = PowerPools.BrainstoneLocation.Bowl1;
				var normalTokensToSpend = powerToSpend - BrainstonePowerValue;
				power.Bowl3 -= normalTokensToSpend;
				power.Bowl1 += normalTokensToSpend;
			}
			else
			{
				power.Bowl3 -= powerToSpend;
				power.Bowl1 += powerToSpend;
			}
			return state;
		}

		private static PlayerState ApplyCost(PowerTokensCost cost, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var powerPools = state.Resources.Power;
			powerPools.Bowl1 -= cost.Bowl1;
			powerPools.Bowl2 -= cost.Bowl2;
			powerPools.Bowl3 -= cost.Bowl3;
			if (cost.MoveToGaiaArea)
			{
				powerPools.GaiaArea += cost.Bowl1 + cost.Bowl2 + cost.Bowl3;
				if (cost.Brainstone)
				{
					powerPools.Brainstone = PowerPools.BrainstoneLocation.GaiaArea;
				}
			}
			else
			{
				powerPools.GaiaArea -= cost.Gaia;
				if (cost.Brainstone)
				{
					powerPools.Brainstone = PowerPools.BrainstoneLocation.Removed;
				}
			}
			return state;
		}

		private static PlayerState ApplyCost(UseFederationTokenCost tokenCost, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var federationToUse = state.FederationTokens.First(fed => !fed.UsedForTechOrAdvancedTile);
			federationToUse.UsedForTechOrAdvancedTile = true;
			return state;
		}

		#endregion

		#region Gains

		/// <summary>
		/// Applies a gain to a player's state. Returns the new player's state.
		/// </summary>
		/// <param name="gain">The gain to apply</param>
		/// <param name="ctx">The action context</param>
		internal static PlayerState ApplyGain(Gain gain, ActionContext ctx)
		{
			return gain switch
			{
				ResourcesGain rg => ApplyGain(rg, ctx),
				PowerGain pwg => ApplyGain(pwg, ctx),
				TerraformationRatioGain trg => ApplyGain(trg, ctx),
				NavigationRangeGain nrg => ApplyGain(nrg, ctx),
				PointsGain ptg => ApplyGain(ptg, ctx),
				ResearchStepGain rsg => ApplyGain(rsg, ctx),
				UnlockGaiaformerGain ugf => ApplyGain(ugf, ctx),
				FederationTokenGain fg => ApplyGain(fg, ctx),
				TempTerraformationStepsGain tsg => ApplyGain(tsg, ctx),
				RangeBoostGain rbg => ApplyGain(rbg, ctx),
				_ => throw new Exception($"Gain of type {gain.Type} not handled.")
			};
		}

		private static PlayerState ApplyGain(ResourcesGain gain, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var playersRace = ctx.Player.RaceId.Value;
			var playersResources = state.Resources;
			playersResources.Credits = Math.Clamp(playersResources.Credits + gain.Credits, 0, MaxCredits);
			playersResources.Knowledge = Math.Clamp(playersResources.Knowledge + gain.Knowledge, 0, MaxKnowledge);
			var oresToGain = gain.Ores;
			var qicToGain = gain.Qic;
			var isGleensWithoutAcademy = playersRace == Race.Gleens && !state.Buildings.AcademyRight;
			if (isGleensWithoutAcademy)
			{
				oresToGain += gain.Qic;
				qicToGain = 0;
			}
			playersResources.Ores = Math.Clamp(playersResources.Ores + oresToGain, 0, MaxOres);
			playersResources.Qic += qicToGain;
			if (gain.PowerTokens > 0)
			{
				var tokens = gain.PowerTokens;
				var power = playersResources.Power;
				var involvesBrainstone = playersRace == Race.Taklons && power.Brainstone == PowerPools.BrainstoneLocation.Removed;
				if (involvesBrainstone)
				{
					power.Brainstone = PowerPools.BrainstoneLocation.Bowl1;
					tokens -= 1;
				}
				power.Bowl1 += tokens;
			}
			return state;
		}

		private static PlayerState ApplyGain(PowerGain gain, ActionContext ctx)
		{
			// Power gain might be obtained automatically by players other than the active one (in case of 1x0)
			var playerId = gain.PlayerId ?? ctx.Player.Id;
			var player = ctx.Game.Players.First(p => p.Id == playerId);
			var involvesBrainstone = player.RaceId == Race.Taklons &&
				(player.State.Resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl1 || player.State.Resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl2);
			var state = player.State.Clone();
			var power = state.Resources.Power;
			var powerToGain = gain.Power;
			if (gain.SpentPoints.HasValue)
			{
				state.Points -= gain.SpentPoints.Value;
			}

			/* Currently commented because part of an old implementation idea that will probably never see the light
             * The idea was to let the user choose how to move his power tokens (and therefore choose whether or not to
             * move the brainstone) but it's actually never a good idea not to move the brainstone when you can
            if (gain.Brainstone > 0)
            {
                if (gain.Brainstone == 1)
                {
                    resources.Power.Brainstone = resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl1
                        ? PowerPools.BrainstoneLocation.Bowl2
                        : resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl2
                        ? PowerPools.BrainstoneLocation.Bowl3
                        : throw new Exception("Brainstone cannot move 1 step if not in bowl 1 or 2.");
                }
                if (gain.Brainstone == 2)
                {
                    resources.Power.Brainstone = resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl1
                        ? PowerPools.BrainstoneLocation.Bowl3
                        : throw new Exception("Brainstone cannot move 2 steps if not in bowl 1.");
                }
                throw new Exception("Brainstone cannot move 3 or more steps.");
            }
            */

			if (involvesBrainstone)
			{
				if (power.Brainstone == PowerPools.BrainstoneLocation.Bowl1)
				{
					powerToGain -= 1;
					power.Brainstone = PowerPools.BrainstoneLocation.Bowl2;
				}
			}
			if (powerToGain == 0)
			{
				return state;
			}

			if (power.Bowl1 > 0)
			{
				if (powerToGain <= power.Bowl1)
				{
					power.Bowl1 -= powerToGain;
					power.Bowl2 += powerToGain;
					return state;
				}
				else
				{
					power.Bowl2 += power.Bowl1;
					powerToGain -= power.Bowl1;
					power.Bowl1 = 0;
				}
			}
			if (powerToGain == 0)
			{
				return state;
			}

			if (involvesBrainstone)
			{
				powerToGain -= 1;
				power.Brainstone = PowerPools.BrainstoneLocation.Bowl3;
			}
			if (powerToGain == 0)
			{
				return state;
			}

			if (power.Bowl2 > 0)
			{
				if (powerToGain <= power.Bowl2)
				{
					power.Bowl2 -= powerToGain;
					power.Bowl3 += powerToGain;
					return state;
				}
				else
				{
					power.Bowl3 += power.Bowl2;
					power.Bowl2 = 0;
				}
			}
			return state;
		}

		private static PlayerState ApplyGain(TerraformationRatioGain gain, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			state.TerraformingCost -= 1;
			if (state.TerraformingCost == 0)
			{
				throw new Exception("Terraforming Cost cannot be less than 1.");
			}
			return state;
		}

		private static PlayerState ApplyGain(NavigationRangeGain gain, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			state.Range += 1;
			return state;
		}

		private static PlayerState ApplyGain(PointsGain gain, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			state.Points += gain.Points;
			return state;
		}

		private static PlayerState ApplyGain(ResearchStepGain researchStepGain, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var advancement = state.ResearchAdvancements.Single(adv => adv.Track == researchStepGain.Track);
			advancement.Steps += 1;
			return state;
		}

		private static PlayerState ApplyGain(UnlockGaiaformerGain _, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			var gaiaformerToUnlock = state.Gaiaformers.OrderBy(gf => gf.Id).First(gf => !gf.Unlocked);
			gaiaformerToUnlock.Unlocked = true;
			gaiaformerToUnlock.Available = true;
			return state;
		}

		private static PlayerState ApplyGain(FederationTokenGain federationTokenGain, ActionContext ctx)
		{
			var tempCtx = ApplyMultipleGains(federationTokenGain.Gains, ctx);
			var state = tempCtx.PlayerState;
			state.FederationTokens.Add(FederationToken.FromBonus(federationTokenGain.Token));
			return state;
		}

		private static PlayerState ApplyGain(TempTerraformationStepsGain tempTerraformationSteps, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			state.TempTerraformationSteps = tempTerraformationSteps.Steps;
			return state;
		}

		private static PlayerState ApplyGain(RangeBoostGain rangeBoost, ActionContext ctx)
		{
			var state = ctx.PlayerState.Clone();
			state.RangeBoost ??= 0;
			state.RangeBoost += rangeBoost.Boost;
			return state;
		}

		private static ActionContext ApplyMultipleGains(IEnumerable<Gain> gains, ActionContext ctx)
		{
			var tempCtx = ctx.Clone();
			foreach (var gain in gains)
			{
				var state = ApplyGain(gain, tempCtx);
				tempCtx.SetPlayerState(state);
			}
			return tempCtx;
		}

		#endregion
	}
}