using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class UseTechnologyTileActionHandler : ActionHandlerBase<UseTechnologyTileAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, UseTechnologyTileAction action)
		{
			var effects = new List<Effect>
			{
				new SpecialActionUsedEffect(
					action.TileId,
					action.Advanced
						? SpecialActionType.AdvancedTechnologyTile
						: SpecialActionType.StandardTechnologyTile
				)
			};

			Gain gain;
			if (action.Advanced)
			{
				var resources = (AdvancedTechnologyTileType)action.TileId switch
				{
					AdvancedTechnologyTileType.ActionGain1Qic5Credits => new Resources { Credits = 5, Qic = 1 },
					AdvancedTechnologyTileType.ActionGain3Knowledge => new Resources { Knowledge = 3 },
					AdvancedTechnologyTileType.ActionGain3Ores => new Resources { Ores = 3 },
					_ => throw new Exception("Impossible")
				};
				gain = new ResourcesGain(resources, "Technology Tile");
			}
			else
			{
				gain = (StandardTechnologyTileType)action.TileId switch
				{
					StandardTechnologyTileType.ActionGain4Power => new PowerGain(4),
					_ => throw new Exception("Impossible")
				};
			}
			effects.Add(gain);
			effects.Add(new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()));
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, UseTechnologyTileAction action)
		{
			if (!HasTile(action.TileId, action.Advanced))
			{
				return (false, "The selected tile was already used in this round");
			}
			if (!IsTileAvailable(action.TileId, action.Advanced))
			{
				return (false, "The selected tile was already used in this round");
			}
			return (true, null);
		}

		#region Validation

		private bool HasTile(int id, bool advanced)
		{
			return advanced
				? Player.State.AdvancedTechnologyTiles.Any(adv => adv.Id == (AdvancedTechnologyTileType)id)
				: Player.State.StandardTechnologyTiles.Any(std => std.Id == (StandardTechnologyTileType)id);
		}

		private bool IsTileAvailable(int id, bool advanced)
		{
			return !(advanced
				? Player.State.AdvancedTechnologyTiles.Single(adv => adv.Id == (AdvancedTechnologyTileType)id).Used
				: Player.State.StandardTechnologyTiles.Single(std => std.Id == (StandardTechnologyTileType)id).Used);
		}

		#endregion
	}
}