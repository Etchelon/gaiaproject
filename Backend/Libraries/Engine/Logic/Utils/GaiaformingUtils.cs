using System;
using System.Linq;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class GaiaformingUtils
	{
		public static PowerTokensCost GetActualCostForGaiaProject(PlayerInGame player)
		{
			var unlockedGaiaformers = player.State.Gaiaformers.Count(gf => gf.Unlocked);
			var powerTokensToMoveToGaiaArea = unlockedGaiaformers switch
			{
				1 => 6,
				2 => 4,
				3 => 3,
				_ => throw new ArgumentOutOfRangeException(nameof(unlockedGaiaformers))
			};
			return PowerTokensCost.ToGaiaArea(powerTokensToMoveToGaiaArea, player);
		}

		public static bool CanStartGaiaProject(PlayerInGame player, GaiaProjectGame game)
		{
			var cost = GetActualCostForGaiaProject(player);
			return ResourceUtils.CanPayCost(cost, new ActionContext(new NullAction { PlayerId = player.Id }, game), out _);
		}

		public static PlayerState UnlockGaiaformer(PlayerInGame player)
		{
			var playerState = player.State.Clone();
			var gaiaformerToUnlock = playerState.Gaiaformers.OrderBy(gf => gf.Id).FirstOrDefault(gf => !gf.Unlocked);
			if (gaiaformerToUnlock == null)
			{
				throw new Exception($"There are no further gaiaformers to unlock");
			}
			gaiaformerToUnlock.Unlocked = true;
			gaiaformerToUnlock.Available = true;
			gaiaformerToUnlock.OnHexId = null;
			gaiaformerToUnlock.SpentInGaiaArea = false;
			return playerState;
		}

		public static PlayerState ReturnGaiaformerFromHex(PlayerInGame player, string hexId)
		{
			var playerState = player.State.Clone();
			var gaiaformerToMakeAvailable = playerState.Gaiaformers.Single(gf => gf.OnHexId == hexId);
			gaiaformerToMakeAvailable.Available = true;
			gaiaformerToMakeAvailable.OnHexId = null;
			return playerState;
		}

		public static PlayerState SendGaiaformerToHex(PlayerInGame player, string hexId)
		{
			var playerState = player.State.Clone();
			var firstAvailableGf = playerState.Gaiaformers.OrderByDescending(gf => gf.Id).First(gf => gf.Available);
			firstAvailableGf.Available = false;
			firstAvailableGf.OnHexId = hexId;
			return playerState;
		}
	}
}