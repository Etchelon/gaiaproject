using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ChooseTechnologyTileActionHandler : ActionHandlerBase<ChooseTechnologyTileAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, ChooseTechnologyTileAction action)
		{
			var effects = new List<Effect>
			{
				new AcquireTechnologyTileEffect(action.TileId, action.Advanced, action.CoveredTileId)
			};

			var playersBuildings = Player.State.Buildings;
			var mapService = new MapService(game.BoardState.Map);
			bool updateIncomes = false;
			List<Gain> tileGains = new List<Gain>();
			if (action.Advanced)
			{
				effects.Add(new UseFederationTokenCost());

				var tile = (AdvancedTechnologyTileType)action.TileId;

				int numPlayersSectors = Player.State.ColonizedSectors.Count;
				var pointsGain = tile switch
				{
					AdvancedTechnologyTileType.Immediate2PointsPerGaiaPlanet => new PointsGain(Player.State.GaiaPlanets * 2, $"advanced tile {tile}"),
					AdvancedTechnologyTileType.Immediate2PointsPerMine => new PointsGain((playersBuildings.Mines + (playersBuildings.HasLostPlanet ? 1 : 0)) * 2, $"advanced tile {tile}"),
					AdvancedTechnologyTileType.Immediate4PointsPerTradingStation => new PointsGain(playersBuildings.TradingStations * 4, $"advanced tile {tile}"),
					AdvancedTechnologyTileType.Immediate2PointsPerSector => new PointsGain(numPlayersSectors * 2, $"advanced tile {tile}"),
					AdvancedTechnologyTileType.Immediate5PointsPerFederation => new PointsGain(Player.State.FederationTokens.Count * 5, $"advanced tile {tile}"),
					_ => null
				};
				tileGains.Add(pointsGain);

				var resourcesGain = tile switch
				{
					AdvancedTechnologyTileType.Immediate1OrePerSector => new ResourcesGain(new Resources { Ores = numPlayersSectors }),
					_ => null
				};
				tileGains.Add(resourcesGain);

				updateIncomes = action.CoveredTileId switch
				{
					StandardTechnologyTileType.Income1Knowledge1Coin => true,
					StandardTechnologyTileType.Income4Coins => true,
					StandardTechnologyTileType.Income1Ore1Power => true,
					_ => false
				};

				if (action.CoveredTileId == StandardTechnologyTileType.PassiveBigBuildingsWorth4Power)
				{
					effects.Add(new ChangePowerValueOfBigBuildingsEffect(-1));
					var decreaseFederationPowerValueEffects = Player.State.Federations
						.Select(fed =>
						{
							var hexes = fed.HexIds.Select(mapService.GetHex).ToList();
							var decrease = -1 * hexes
								.Select(h => h.Buildings.SingleOrDefault(b => b.PlayerId == Player.Id))
								.Where(b => b.Type == BuildingType.PlanetaryInstitute || b.Type == BuildingType.AcademyLeft || b.Type == BuildingType.AcademyRight)
								.Count();
							return new ChangePowerValueOfFederationEffect(fed.Id, decrease);
						})
						.Where(eff => eff.Variation < 0)
						.ToList();
					effects.AddRange(decreaseFederationPowerValueEffects);
				}
			}
			else
			{
				var tile = (StandardTechnologyTileType)action.TileId;

				var pointsGain = tile switch
				{
					StandardTechnologyTileType.Immediate7Points => new PointsGain(7, $"Standard tile {tile}"),
					_ => null
				};
				tileGains.Add(pointsGain);

				var resourcesGain = tile switch
				{
					StandardTechnologyTileType.Immediate1KnowledgePerPlanetType => new ResourcesGain(new Resources { Knowledge = Player.State.KnownPlanetTypes.Count }),
					StandardTechnologyTileType.Immediate1Ore1Qic => new ResourcesGain(new Resources { Ores = 1, Qic = 1 }),
					_ => null
				};
				tileGains.Add(resourcesGain);

				updateIncomes = tile switch
				{
					StandardTechnologyTileType.Income1Knowledge1Coin => true,
					StandardTechnologyTileType.Income4Coins => true,
					StandardTechnologyTileType.Income1Ore1Power => true,
					_ => false
				};

				if (tile == StandardTechnologyTileType.PassiveBigBuildingsWorth4Power)
				{
					effects.Add(new ChangePowerValueOfBigBuildingsEffect());
					var increaseFederationPowerValueEffects = Player.State.Federations
						.Select(fed =>
						{
							var hexes = fed.HexIds.Select(mapService.GetHex).ToList();
							var increase = hexes
								.Select(h => h.Buildings.SingleOrDefault(b => b.PlayerId == Player.Id))
								.Where(b => b.Type == BuildingType.PlanetaryInstitute || b.Type == BuildingType.AcademyLeft || b.Type == BuildingType.AcademyRight)
								.Count();
							return new ChangePowerValueOfFederationEffect(fed.Id, increase);
						})
						.Where(eff => eff.Variation > 0)
						.ToList();
					effects.AddRange(increaseFederationPowerValueEffects);
				}
			}

			if (updateIncomes)
			{
				effects.Add(new IncomeVariationEffect(IncomeSource.StandardTechnologyTile));
			}
			effects.AddRange(tileGains.Where(g => g != null));

			bool shouldDecideResearchStep = action.Advanced
											|| game.BoardState.ResearchBoard
												.FreeStandardTiles.Any(t => t.Type == (StandardTechnologyTileType)action.TileId);
			if (shouldDecideResearchStep)
			{
				var decision = new FreeTechnologyStepDecision();
				effects.Add(new PendingDecisionEffect(decision));
			}
			else
			{
				var tilesTrack = game.BoardState.ResearchBoard
					.Tracks.Single(t => t.StandardTiles.Type == (StandardTechnologyTileType)action.TileId);
				var playersAdvancement = Player.State.ResearchAdvancements.Single(adv => adv.Track == tilesTrack.Id);
				var (canAdvance, _) = ResearchUtils.CanPlayerAdvanceToLevel(
					playersAdvancement.Steps + 1, tilesTrack.Id, Player.Id, game);

				PendingDecision actualDecision = new PerformConversionOrPassTurnDecision();
				if (canAdvance)
				{
					// If the tile would cause the player to spend one of his federation tokens, ask for confirmation
					if (playersAdvancement.Steps == ResearchUtils.MaxSteps - 1)
					{
						actualDecision = AcceptOrDeclineLastStepDecision.Construct(action, tilesTrack.Id);
					}
					else
					{
						var stepEffects = ResearchUtils.ApplyStep(tilesTrack.Id, Player.Id, Game, true);
						var withoutDecision = stepEffects.Where(eff => !(eff is PendingDecisionEffect));
						var decisionEffect = stepEffects.OfType<PendingDecisionEffect>().SingleOrDefault();
						effects.AddRange(withoutDecision);
						if (decisionEffect != null)
						{
							actualDecision = decisionEffect.Decision;
						}
					}
				}
				else
				{
					effects.Add(new LogEffect($"cannot advance any further in track {tilesTrack.Id}"));
				}

				if (tilesTrack.Id == ResearchTrackType.Economy || tilesTrack.Id == ResearchTrackType.Science)
				{
					effects.Add(new IncomeVariationEffect(tilesTrack.Id == ResearchTrackType.Economy
						? IncomeSource.EconomyTrack
						: IncomeSource.ScienceTrack)
					);
				}
				effects.Add(new PendingDecisionEffect(actualDecision));
			}

			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ChooseTechnologyTileAction action)
		{
			if (!TileIsAvailable(action))
			{
				return (false, "The selected tile is not available");
			}
			if (AlreadyHasTile(action))
			{
				return (false, "You already have this tile");
			}

			if (action.Advanced && !CanCoverTile(action.CoveredTileId!.Value, out var reason))
			{
				return (false, $"You cannot cover tile {action.CoveredTileId} because {reason}");
			}
			return (true, null);
		}

		#region Validation

		private bool TileIsAvailable(ChooseTechnologyTileAction action)
		{
			if (!action.Advanced)
			{
				return true;
			}

			var availableTiles = Game.BoardState.ResearchBoard
				.Tracks
				.Where(t => t.IsAdvancedTileAvailable)
				.Select(t => t.AdvancedTileType)
				.ToList();
			return availableTiles.Any(t => t == (AdvancedTechnologyTileType)action.TileId);
		}

		private bool AlreadyHasTile(ChooseTechnologyTileAction action)
		{
			if (action.Advanced)
			{
				return false;
			}

			var playerTiles = Player.State.StandardTechnologyTiles;
			return playerTiles.Any(t => t.Id == (StandardTechnologyTileType)action.TileId);
		}

		private bool CanCoverTile(StandardTechnologyTileType tile, out string reason)
		{
			var standardTile = Player.State.StandardTechnologyTiles.SingleOrDefault(t => t.Id == tile);
			if (standardTile == null)
			{
				reason = $"you do not have tile {tile}";
				return false;
			}
			if (standardTile.CoveredByAdvancedTile)
			{
				reason = $"tile {tile} is already covered by another advanced tile";
				return false;
			}

			reason = null;
			return true;
		}

		#endregion
	}
}