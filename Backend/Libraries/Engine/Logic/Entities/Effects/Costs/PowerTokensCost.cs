using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects.Costs
{
	public class PowerTokensCost : Cost
	{
		public override CostType Type => CostType.PowerTokens;
		/// <summary>
		/// Whether to move the tokens to the Gaia area or to just remove them
		/// </summary>
		public bool MoveToGaiaArea { get; private set; }
		/// <summary>
		/// How many power tokens to remove from Bowl 1
		/// </summary>
		public int Bowl1 { get; }
		/// <summary>
		/// How many power tokens to remove from Bowl 2
		/// </summary>
		public int Bowl2 { get; }
		/// <summary>
		/// How many power tokens to remove from Bowl 3
		/// </summary>
		public int Bowl3 { get; }
		/// <summary>
		/// How many power tokens to remove from Gaia Area
		/// </summary>
		public int Gaia { get; }
		/// <summary>
		/// Whether to move the brainstone
		/// </summary>
		public bool Brainstone { get; }

		private PowerTokensCost(int bowl1, int bowl2, int bowl3, int gaia, bool brainstone = false)
		{
			Bowl1 = bowl1;
			Bowl2 = bowl2;
			Bowl3 = bowl3;
			Gaia = gaia;
			Brainstone = brainstone;
		}

		private static PowerTokensCost Create(int total, PlayerInGame player)
		{
			var power = player.State.Resources.Power;
			var fromBowl1 = 0;
			var fromBowl2 = 0;
			var fromBowl3 = 0;
			bool brainstone = false;

			var remainder = total;
			if (power.Bowl1 >= remainder)
			{
				fromBowl1 = remainder;
				remainder = 0;
			}
			else
			{
				fromBowl1 = power.Bowl1;
				remainder -= fromBowl1;
			}
			if (power.Bowl2 >= remainder)
			{
				fromBowl2 = remainder;
				remainder = 0;
			}
			else
			{
				fromBowl2 = power.Bowl2;
				remainder -= fromBowl2;
			}
			if (power.Bowl3 >= remainder)
			{
				fromBowl3 = remainder;
				remainder = 0;
			}
			else
			{
				fromBowl3 = power.Bowl3;
				remainder -= fromBowl3;
			}

			if (remainder > 1 || (remainder > 0 && player.RaceId != Race.Taklons))
			{
				// Player cannot pay the cost
				return new PowerTokensCost(99, 99, 99, 0) { MoveToGaiaArea = true };
			}
			if (remainder == 1 && player.RaceId == Race.Taklons)
			{
				brainstone = true;
			}
			return new PowerTokensCost(fromBowl1, fromBowl2, fromBowl3, 0, brainstone);
		}

		public static PowerTokensCost Remove(int total, PlayerInGame player)
		{
			var ret = Create(total, player);
			ret.MoveToGaiaArea = false;
			return ret;
		}

		public static PowerTokensCost ToGaiaArea(int total, PlayerInGame player)
		{
			var ret = Create(total, player);
			ret.MoveToGaiaArea = true;
			return ret;
		}

		public static PowerTokensCost RemoveFromGaiaArea(int total)
		{
			return new PowerTokensCost(0, 0, 0, total);
		}

		public override string ToString()
		{
			var msg = "";
			if (Bowl1 > 0)
			{
				msg += $"{Bowl1} from bowl 1";
			}
			if (Bowl2 > 0)
			{
				msg += string.IsNullOrEmpty(msg) ? "" : ", ";
				msg += $"{Bowl2} from bowl 2";
			}
			if (Bowl3 > 0)
			{
				msg += string.IsNullOrEmpty(msg) ? "" : ", ";
				msg += $"{Bowl3} from bowl 3";
			}
			if (Brainstone)
			{
				msg += string.IsNullOrEmpty(msg) ? "" : ", ";
				msg += $"removes the brainstone";
			}
			return $"power tokens, which are {(MoveToGaiaArea ? "moved to Gaia Area" : "removed from the player's board")}: {msg}";
		}
	}
}
