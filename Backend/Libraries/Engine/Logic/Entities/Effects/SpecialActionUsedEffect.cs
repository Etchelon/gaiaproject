using System;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class SpecialActionUsedEffect : Effect
	{
		public int? ElementId { get; }
		public SpecialActionType Type { get; }

		public SpecialActionUsedEffect(int? elementId, SpecialActionType type)
		{
			ElementId = elementId;
			Type = type;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.CurrentPlayer;
			switch (Type)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), $"Special action of type {Type} not handled.");
				case SpecialActionType.StandardTechnologyTile:
					{
						var tile = player.State.StandardTechnologyTiles.Single(adv =>
						{
							Debug.Assert(ElementId != null, nameof(ElementId) + " != null");
							return adv.Id == (StandardTechnologyTileType)ElementId;
						});
						tile.Used = true;
						break;
					}
				case SpecialActionType.AdvancedTechnologyTile:
					{
						var tile = player.State.AdvancedTechnologyTiles.Single(adv =>
						{
							Debug.Assert(ElementId != null, nameof(ElementId) + " != null");
							return adv.Id == (AdvancedTechnologyTileType)ElementId;
						});
						tile.Used = true;
						break;
					}
				case SpecialActionType.RoundBooster:
					{
						var booster = player.State.RoundBooster;
						booster.Used = true;
						break;
					}
				case SpecialActionType.PlanetaryInstitute:
					{
						player.Actions.HasUsedPlanetaryInstitute = true;
						break;
					}
				case SpecialActionType.RightAcademy:
					{
						player.Actions.HasUsedRightAcademy = true;
						break;
					}
				case SpecialActionType.RaceAction:
					{
						player.Actions.HasUsedRaceAction = true;
						break;
					}
			}
		}
	}
}