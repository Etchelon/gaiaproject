using System.Linq;
using System.Text;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class ConversionEffect : Effect
	{
		public int NetCreditsVariation { get; set; }
		public int NetOresVariation { get; set; }
		public int NetKnowledgeVariation { get; set; }
		public int NetQicVariation { get; set; }
		public int BurnedPower { get; set; }
		public int SpentPower { get; set; }
		public PowerPools.BrainstoneLocation? BrainstoneLocation { get; set; }
		public int NewPowerTokens { get; set; }
		public int PowerMovedToGaiaArea { get; set; }
		public int GaiaformersMovedToGaiaArea { get; set; }
		public int RangeBoost { get; set; }

		public ConversionEffect(int netCreditsVariation, int netOresVariation, int netKnowledgeVariation, int netQicVariation, int burnedPower, int spentPower, PowerPools.BrainstoneLocation? brainstoneLocation, int newPowerTokens, int powerMovedToGaiaArea, int gaiaformersMovedToGaiaArea, int rangeBoost)
		{
			NetCreditsVariation = netCreditsVariation;
			NetOresVariation = netOresVariation;
			NetKnowledgeVariation = netKnowledgeVariation;
			NetQicVariation = netQicVariation;
			BurnedPower = burnedPower;
			SpentPower = spentPower;
			BrainstoneLocation = brainstoneLocation;
			NewPowerTokens = newPowerTokens;
			PowerMovedToGaiaArea = powerMovedToGaiaArea;
			GaiaformersMovedToGaiaArea = gaiaformersMovedToGaiaArea;
			RangeBoost = rangeBoost;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);

			var sb = new StringBuilder("performs conversions.");

			var hadBrainstoneInBowl2 = player.State.Resources.Power.Brainstone == PowerPools.BrainstoneLocation.Bowl2;
			if (BurnedPower > 0)
			{
				// If at the beginning of the action the brainstone is in Bowl2, 1 less power needs to be burned as the brainstone is moved instead of a power token
				player.State.Resources.Power.Bowl2 -= 2 * BurnedPower - (hadBrainstoneInBowl2 ? 1 : 0);
				player.State.Resources.Power.Bowl3 += BurnedPower - (hadBrainstoneInBowl2 ? 1 : 0);
				if (hadBrainstoneInBowl2)
				{
					player.State.Resources.Power.Brainstone = PowerPools.BrainstoneLocation.Bowl3;
				}
				if (player.RaceId == Race.Itars)
				{
					player.State.Resources.Power.GaiaArea += BurnedPower;
				}
				sb.Append($" Burns {BurnedPower} power.");
			}

			if (PowerMovedToGaiaArea > 0)
			{
				player.State.Resources.Power.Bowl3 -= PowerMovedToGaiaArea;
				player.State.Resources.Power.GaiaArea += PowerMovedToGaiaArea;
				sb.Append($" Moves {PowerMovedToGaiaArea} power from Bowl 3 to Gaia Area.");
			}

			if (GaiaformersMovedToGaiaArea > 0)
			{
				player.State.Gaiaformers
					.Where(gf => gf.Available)
					.Take(GaiaformersMovedToGaiaArea)
					.ToList()
					.ForEach(gf =>
					{
						gf.Available = false;
						gf.SpentInGaiaArea = true;
					});
				sb.Append($" Converts {GaiaformersMovedToGaiaArea} gaiaformers to Qic.");
			}

			var spentResources = new Resources
			{
				Power = SpentPower
			};
			var earnedResources = new Resources
			{
				PowerTokens = NewPowerTokens
			};
			if (NetCreditsVariation < 0)
			{
				spentResources.Credits = -NetCreditsVariation;
			}
			else if (NetCreditsVariation > 0)
			{
				earnedResources.Credits = NetCreditsVariation;
			}
			if (NetOresVariation < 0)
			{
				spentResources.Ores = -NetOresVariation;
			}
			else if (NetOresVariation > 0)
			{
				earnedResources.Ores = NetOresVariation;
			}
			if (NetKnowledgeVariation < 0)
			{
				spentResources.Knowledge = -NetKnowledgeVariation;
			}
			else if (NetKnowledgeVariation > 0)
			{
				earnedResources.Knowledge = NetKnowledgeVariation;
			}
			if (NetQicVariation < 0)
			{
				spentResources.Qic = -NetQicVariation;
			}
			else if (NetQicVariation > 0)
			{
				earnedResources.Qic = NetQicVariation;
			}

			var cost = new ResourcesCost(spentResources);
			var hasSpentResources = cost.ToString() != null;
			if (hasSpentResources)
			{
				player.State = ResourceUtils.ApplyCost(
					cost,
					new ActionContext(new NullAction { PlayerId = PlayerId }, game)
				);
				sb.Append($" Spends {cost}");
			}

			var gain = new ResourcesGain(earnedResources);
			var hasGainedResources = gain.ToString() != null;
			if (hasGainedResources)
			{
				player.State = ResourceUtils.ApplyGain(
					gain,
					new ActionContext(new NullAction { PlayerId = PlayerId }, game)
				);
				sb.Append($" to earn {gain}");
			}

			if (RangeBoost > 0)
			{
				player.State.RangeBoost ??= 0;
				player.State.RangeBoost += RangeBoost;
				sb.Append($"{(hasGainedResources ? " and" : "")} to boost its range by {RangeBoost}");
			}

			var message = sb.ToString();
			game.LogEffect(this, message);
		}
	}
}