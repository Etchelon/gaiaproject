using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Utils
{
	internal static class IncomeUtils
	{
		private static List<StandardTechnologyTileType> _standardTilesWithIncome = new List<StandardTechnologyTileType>
		{
			StandardTechnologyTileType.Income1Knowledge1Coin,
			StandardTechnologyTileType.Income1Ore1Power,
			StandardTechnologyTileType.Income4Coins,
		};

		public static List<Income> UpdateIncomeFrom(IncomeSource source, PlayerInGame player)
		{
			var playerState = player.State.Clone();
			switch (source)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(source), $"Income source {source} not handled.");
				case IncomeSource.Base:
					throw new Exception("Base income cannot vary during the game.");
				case IncomeSource.Buildings:
					return UpdateIncomeFromBuildings(player.RaceId.Value, playerState);
				case IncomeSource.EconomyTrack:
					{
						var steps = playerState.ResearchAdvancements.First(adv => adv.Track == ResearchTrackType.Economy).Steps;
						return UpdateIncomeFromEconomyTrack(steps, playerState);
					}
				case IncomeSource.ScienceTrack:
					{
						var steps = playerState.ResearchAdvancements.First(adv => adv.Track == ResearchTrackType.Science).Steps;
						return UpdateIncomeFromScienceTrack(steps, playerState);
					}
				case IncomeSource.RoundBooster:
					var roundBooster = player.State.RoundBooster;
					if (roundBooster == null)
					{
						throw new Exception($"Player {player.Username} does not have a round booster.");
					}
					return UpdateIncomeFromRoundBooster(roundBooster.Id, playerState);
				case IncomeSource.StandardTechnologyTile:
					var standardTiles = player.State.StandardTechnologyTiles
						.Where(t => !t.CoveredByAdvancedTile && _standardTilesWithIncome.Any(twi => twi == t.Id))
						.Select(t => t.Id)
						.ToList();
					return UpdateIncomeFromStandardTiles(standardTiles, playerState);
			}
		}

		#region Private logic

		internal static List<Income> GetBaseIncomes(Race race)
		{
			var ret = new List<Income>
			{
				new ResourceIncome
				{
					SourceType = IncomeSource.Base,
					Credits = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.BaseCreditsIncome),
					Ores = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.BaseOresIncome),
					Knowledge = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.BaseKnowledgeIncome),
					Qic = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.BaseQicIncome),
				}
			};
			var powerTokens = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.BasePowerTokensIncome);
			if (powerTokens > 0)
			{
				ret.Add(new PowerTokenIncome { SourceType = IncomeSource.Base, PowerTokens = powerTokens });
			}
			var initialStep = RaceUtils.GetInitialResearchStep(race);
			if (initialStep == ResearchTrackType.Economy)
			{
				ret.Add(new ResourceIncome { Credits = 2, SourceType = IncomeSource.EconomyTrack });
				ret.Add(new PowerIncome { Power = 1, SourceType = IncomeSource.EconomyTrack });
			}
			if (initialStep == ResearchTrackType.Science)
			{
				ret.Add(new ResourceIncome { Knowledge = 1, SourceType = IncomeSource.ScienceTrack });
			}
			return ret;
		}

		private static List<Income> FromBuildings(Race race, DeployedBuildings buildings)
		{
			var ret = new List<Income>();

			var resourcesIncome = new ResourceIncome
			{
				SourceType = IncomeSource.Buildings
			};
			// Credits
			{
				var creditsFromTradingStations = new Dictionary<int, int>
				{
					{ 0, 0 },
					{ 1, 3 },
					{ 2, 7 },
					{ 3, 11 },
					{ 4, 16 },
				};
				var creditsFromBescodsResearchLabs = new Dictionary<int, int>
				{
					{ 0, 0 },
					{ 1, 3 },
					{ 2, 7 },
					{ 3, 12 },
				};
				var isBescods = race == Race.Bescods;
				resourcesIncome.Credits = isBescods
					? creditsFromBescodsResearchLabs[buildings.ResearchLabs]
					: creditsFromTradingStations[buildings.TradingStations];
			}
			// Ores
			{
				var oresFromMines = new Dictionary<int, int>
				{
					{ 0, 0 },
					{ 1, 1 },
					{ 2, 2 },
					{ 3, 2 },
					{ 4, 3 },
					{ 5, 4 },
					{ 6, 5 },
					{ 7, 6 },
					{ 8, 7 },
				};
				var isGleen = race == Race.Gleens;
				resourcesIncome.Ores = oresFromMines[buildings.Mines] + (isGleen && buildings.PlanetaryInstitute ? 1 : 0);
			}
			// Knowledge
			{
				var isBescods = race == Race.Bescods;
				var isNevlas = race == Race.Nevlas;
				resourcesIncome.Knowledge = isBescods
					? buildings.TradingStations
					: (isNevlas ? 0 : buildings.ResearchLabs);
				if (buildings.AcademyLeft)
				{
					var isItars = race == Race.Itars;
					resourcesIncome.Knowledge += isItars ? 3 : 2;
				}
			}
			// Qic
			{
				var isXenos = race == Race.Xenos;
				resourcesIncome.Qic = isXenos && buildings.PlanetaryInstitute ? 1 : 0;
			}

			ret.Add(resourcesIncome);

			// Power
			{
				var powerIncomes = new List<PowerIncome>();
				if (buildings.PlanetaryInstitute)
				{
					powerIncomes.Add(new PowerIncome { SourceType = IncomeSource.Buildings, Power = 4 });
				}
				var isNevlas = race == Race.Nevlas;
				if (isNevlas)
				{
					var powerFromResearchLabs = Enumerable.Repeat(
						new PowerIncome { SourceType = IncomeSource.Buildings, Power = 2 },
						buildings.ResearchLabs
					).ToList();
					powerIncomes.AddRange(powerFromResearchLabs);
				}

				ret.AddRange(powerIncomes);
			}

			// Power Tokens
			{
				var powerTokensFromPlanetaryInstitutes = new Dictionary<Race, int>
				{
					{ Race.Ambas, 2 },
					{ Race.BalTaks, 1 },
					{ Race.Bescods, 2 },
					{ Race.Firaks, 1 },
					{ Race.Geodens, 1 },
					{ Race.Gleens, 0 },
					{ Race.HadschHallas, 1 },
					{ Race.Itars, 1 },
					{ Race.Ivits, 1 },
					{ Race.Lantids, 0 },
					{ Race.Nevlas, 1 },
					{ Race.Taklons, 1 },
					{ Race.Terrans, 1 },
					{ Race.Xenos, 0 },
				};
				if (buildings.PlanetaryInstitute)
				{
					var tokens = powerTokensFromPlanetaryInstitutes[race];
					if (tokens > 0)
					{
						ret.Add(new PowerTokenIncome
						{
							SourceType = IncomeSource.Buildings,
							PowerTokens = tokens
						});
					}
				}
			}

			return ret;
		}

		private static List<Income> FromStandardTechnologyTile(StandardTechnologyTileType tile)
		{
			var incomes = new List<Income>();
			switch (tile)
			{
				default:
					throw new Exception($"Tech tile type {tile} not handled.");
				case StandardTechnologyTileType.Income1Knowledge1Coin:
					incomes.Add(new ResourceIncome
					{
						Credits = 1,
						Knowledge = 1
					});
					break;
				case StandardTechnologyTileType.Income1Ore1Power:
					incomes.Add(new ResourceIncome
					{
						Ores = 1,
					});
					incomes.Add(new PowerIncome
					{
						Power = 1,
					});
					break;
				case StandardTechnologyTileType.Income4Coins:
					incomes.Add(new ResourceIncome
					{
						Credits = 4,
					});
					break;
			}
			return incomes
				.Select(inc =>
				{
					inc.SourceId = tile.ToString();
					inc.SourceType = IncomeSource.StandardTechnologyTile;
					return inc;
				})
				.ToList();
		}

		private static List<Income> FromBooster(RoundBoosterType booster)
		{
			var incomes = new List<Income>();
			switch (booster)
			{
				default:
					throw new Exception($"Booster type {booster} not handled.");
				case RoundBoosterType.BoostRangeGainPower:
					incomes.Add(new PowerIncome { Power = 2 });
					break;
				case RoundBoosterType.GainCreditsGainQic:
					incomes.Add(new ResourceIncome { Credits = 2, Qic = 1 });
					break;
				case RoundBoosterType.GainOreGainKnowledge:
					incomes.Add(new ResourceIncome { Ores = 1, Knowledge = 1 });
					break;
				case RoundBoosterType.GainPowerTokensGainOre:
					incomes.Add(new ResourceIncome { Ores = 1 });
					incomes.Add(new PowerTokenIncome { PowerTokens = 2 });
					break;
				case RoundBoosterType.PassPointsPerBigBuildingsGainPower:
					incomes.Add(new PowerIncome { Power = 4 });
					break;
				case RoundBoosterType.PassPointsPerGaiaPlanetsGainCredits:
					incomes.Add(new ResourceIncome { Credits = 4 });
					break;
				case RoundBoosterType.PassPointsPerMineGainOre:
					incomes.Add(new ResourceIncome { Ores = 1 });
					break;
				case RoundBoosterType.PassPointsPerResearchLabsGainKnowledge:
					incomes.Add(new ResourceIncome { Knowledge = 1 });
					break;
				case RoundBoosterType.PassPointsPerTradingStationsGainOre:
					incomes.Add(new ResourceIncome { Ores = 1 });
					break;
				case RoundBoosterType.TerraformActionGainCredits:
					incomes.Add(new ResourceIncome { Credits = 2 });
					break;
			}
			return incomes
				.Select(inc =>
				{
					inc.SourceId = booster.ToString();
					inc.SourceType = IncomeSource.RoundBooster;
					return inc;
				})
				.ToList();
		}

		private static List<Income> FromEconomyTrack(int steps)
		{
			var incomes = new List<Income>();
			if (steps == 0 || steps == 5)
			{
				return incomes;
			}
			if (steps == 1)
			{
				incomes.Add(new ResourceIncome { Credits = 2 });
				incomes.Add(new PowerIncome { Power = 1 });
			}
			if (steps == 2)
			{
				incomes.Add(new ResourceIncome { Credits = 2, Ores = 1 });
				incomes.Add(new PowerIncome { Power = 2 });
			}
			if (steps == 3)
			{
				incomes.Add(new ResourceIncome { Credits = 3, Ores = 1 });
				incomes.Add(new PowerIncome { Power = 3 });
			}
			if (steps == 4)
			{
				incomes.Add(new ResourceIncome { Credits = 4, Ores = 2 });
				incomes.Add(new PowerIncome { Power = 4 });
			}
			return incomes
				.Select(inc =>
				{
					inc.SourceType = IncomeSource.EconomyTrack;
					return inc;
				})
				.ToList();
		}

		private static List<Income> FromScienceTrack(int steps)
		{
			return new List<Income>
			{
				new ResourceIncome
				{
					SourceType = IncomeSource.ScienceTrack,
					Knowledge = 0 <= steps && steps <= 4 ? steps : 0
				}
			};
		}

		private static List<Income> UpdateIncomeFromBuildings(Race raceId, PlayerState playerState)
		{
			var playerIncomes = playerState.Incomes.Where(inc => inc.SourceType != IncomeSource.Buildings).ToList();
			var fromBuildings = FromBuildings(raceId, playerState.Buildings);
			playerIncomes.AddRange(fromBuildings);
			return playerIncomes;
		}

		private static List<Income> UpdateIncomeFromEconomyTrack(int steps, PlayerState playerState)
		{
			var playerIncomes = playerState.Incomes.Where(inc => inc.SourceType != IncomeSource.EconomyTrack).ToList();
			playerIncomes.AddRange(FromEconomyTrack(steps));
			return playerIncomes;
		}

		private static List<Income> UpdateIncomeFromScienceTrack(int steps, PlayerState playerState)
		{
			var playerIncomes = playerState.Incomes.Where(inc => inc.SourceType != IncomeSource.ScienceTrack).ToList();
			playerIncomes.AddRange(FromScienceTrack(steps));
			return playerIncomes;
		}

		private static List<Income> UpdateIncomeFromRoundBooster(RoundBoosterType type, PlayerState playerState)
		{
			var playerIncomes = playerState.Incomes.Where(inc => inc.SourceType != IncomeSource.RoundBooster).ToList();
			playerIncomes.AddRange(FromBooster(type));
			return playerIncomes;
		}

		private static List<Income> UpdateIncomeFromStandardTiles(List<StandardTechnologyTileType> tiles, PlayerState playerState)
		{
			var playerIncomes = playerState.Incomes.Where(inc => inc.SourceType != IncomeSource.StandardTechnologyTile).ToList();
			playerIncomes.AddRange(tiles.SelectMany(t => FromStandardTechnologyTile(t)));
			return playerIncomes;
		}

		#endregion
	}
}