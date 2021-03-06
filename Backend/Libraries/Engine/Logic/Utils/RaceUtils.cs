using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class RaceUtils
	{
		public class ActionInfo
		{
			public ActionType Id { get; set; }
			public string Name { get; set; }
		}

		public static List<Race> RandomizeRaces(int nPlayers)
		{
			// Shuffle colors and take N of them (where N is the number of players)
			return Enumerable.Range(1, 7)
				.ToList()
				.Shuffle()
				.Take(nPlayers)
				.Select(color =>
				{
					// Now choose which race to use for this color
					var odd = ThreadSafeRandom.ThisThreadsRandom.Next(0, 2);
					var raceId = color * 2 - odd;
					return (Race)raceId;
				})
				.ToList();
		}

		public static string GetName(Race? race)
		{
			if (!race.HasValue)
			{
				return "";
			}
			return Enum.GetName(typeof(Race), race.Value);
		}

		public static string GetColor(Race race)
		{
			return race switch
			{
				Race.None => "transparent",
				Race.Terrans => "blue",
				Race.Lantids => "blue",
				Race.Ambas => "brown",
				Race.Taklons => "brown",
				Race.Geodens => "orange",
				Race.BalTaks => "orange",
				Race.Ivits => "red",
				Race.HadschHallas => "red",
				Race.Xenos => "yellow",
				Race.Gleens => "yellow",
				Race.Nevlas => "white",
				Race.Itars => "white",
				Race.Bescods => "gray",
				Race.Firaks => "gray",
				_ => "transparent"
			};
		}

		public static PlanetType GetRacePlanetType(Race race)
		{
			return race switch
			{
				Race.Terrans => PlanetType.Terra,
				Race.Lantids => PlanetType.Terra,
				Race.Ambas => PlanetType.Swamp,
				Race.Taklons => PlanetType.Swamp,
				Race.Geodens => PlanetType.Volcanic,
				Race.BalTaks => PlanetType.Volcanic,
				Race.Ivits => PlanetType.Oxide,
				Race.HadschHallas => PlanetType.Oxide,
				Race.Xenos => PlanetType.Desert,
				Race.Gleens => PlanetType.Desert,
				Race.Nevlas => PlanetType.Ice,
				Race.Itars => PlanetType.Ice,
				Race.Bescods => PlanetType.Titanium,
				Race.Firaks => PlanetType.Titanium,
				_ => throw new Exception("Remove race \"None\"")
			};
		}

		public static int GetRaceInitialLevel(Race race, Feature feature)
		{
			return HardcodedValuesForRaces[race][feature];
		}

		public static ResearchTrackType? GetInitialResearchStep(Race race)
		{
			var trackFeatures = new[]
			{
				Feature.InitialTerraformationSteps,
				Feature.InitialNavigationSteps,
				Feature.InitialArtificialIntelligenceSteps,
				Feature.InitialGaiaformationSteps,
				Feature.InitialEconomySteps,
				Feature.InitialScienceSteps,
			};
			var raceValues = HardcodedValuesForRaces[race];
			var trackWithStep = raceValues
				.Where(kvp => trackFeatures.Any(f => f == kvp.Key))
				.Cast<KeyValuePair<Feature, byte>?>()
				.FirstOrDefault(kvp => kvp?.Value == 1);
			if (!trackWithStep.HasValue)
			{
				return null;
			}

			return trackWithStep.Value.Key switch
			{
				Feature.InitialTerraformationSteps => ResearchTrackType.Terraformation,
				Feature.InitialNavigationSteps => ResearchTrackType.Navigation,
				Feature.InitialArtificialIntelligenceSteps => ResearchTrackType.ArtificialIntelligence,
				Feature.InitialGaiaformationSteps => ResearchTrackType.Gaiaformation,
				Feature.InitialEconomySteps => ResearchTrackType.Economy,
				Feature.InitialScienceSteps => ResearchTrackType.Science,
				_ => throw new Exception("Impossible")
			};
		}

		public static ActionInfo GetPlanetaryInstituteActionInfo(PlayerInGame player)
		{
			return player.RaceId switch
			{
				Race.Ambas => new ActionInfo { Id = ActionType.AmbasSwapPlanetaryInstituteAndMine, Name = "Swap" },
				Race.Firaks => new ActionInfo { Id = ActionType.FiraksDowngradeResearchLab, Name = "Downgrade" },
				Race.Ivits => new ActionInfo { Id = ActionType.IvitsPlaceSpaceStation, Name = "Space Station" },
				_ => null
			};
		}

		public static ActionInfo GetRightAcademyActionInfo(PlayerInGame player)
		{
			var isBaltak = player.RaceId == Race.BalTaks;
			return new ActionInfo
			{
				Id = ActionType.UseRightAcademy,
				Name = isBaltak ? "+4 Credits" : "+1 Qic"
			};
		}

		public static ActionInfo GetRaceActionInfo(PlayerInGame player)
		{
			return player.RaceId switch
			{
				Race.Bescods => new ActionInfo
				{
					Id = ActionType.BescodsResearchProgress,
					Name = "Research"
				},
				_ => null
			};
		}

		public enum Feature
		{
			InitialTerraformingCost,
			InitialNavigationRange,
			InitialTerraformationSteps,
			InitialNavigationSteps,
			InitialArtificialIntelligenceSteps,
			InitialGaiaformationSteps,
			InitialEconomySteps,
			InitialScienceSteps,
			InitialCredits,
			InitialOres,
			InitialKnowledge,
			InitialQic,
			InitialPowerBowl1,
			InitialPowerBowl2,
			InitialPowerBowl3,
			BaseCreditsIncome,
			BaseOresIncome,
			BaseKnowledgeIncome,
			BaseQicIncome,
			BasePowerIncome,
			BasePowerTokensIncome,
		}

		#region Race Data

		// TODO: get from file
		private static readonly Dictionary<Race, Dictionary<Feature, byte>> HardcodedValuesForRaces = new Dictionary<Race, Dictionary<Feature, byte>>
		{
			{
				Race.Terrans,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 1 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 4 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Lantids,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 13 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 4 },
					{ Feature.InitialPowerBowl2, 0 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Taklons,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Ambas,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 1 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 2 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Gleens,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 1 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 0 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Xenos,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 1 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Ivits,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 1 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.HadschHallas,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 1 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 3 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Bescods,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 1 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 0 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Firaks,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 3 },
					{ Feature.InitialKnowledge, 2 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 2 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Geodens,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 1 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.BalTaks,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 1 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 0 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 2 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Nevlas,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 1 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 4 },
					{ Feature.InitialKnowledge, 2 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 2 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 0 },
				}
			},
			{
				Race.Itars,
				new Dictionary<Feature, byte>
				{
					{ Feature.InitialTerraformingCost, 3 },
					{ Feature.InitialNavigationRange, 1 },
					{ Feature.InitialTerraformationSteps, 0 },
					{ Feature.InitialNavigationSteps, 0 },
					{ Feature.InitialArtificialIntelligenceSteps, 0 },
					{ Feature.InitialGaiaformationSteps, 0 },
					{ Feature.InitialEconomySteps, 0 },
					{ Feature.InitialScienceSteps, 0 },
					{ Feature.InitialCredits, 15 },
					{ Feature.InitialOres, 5 },
					{ Feature.InitialKnowledge, 3 },
					{ Feature.InitialQic, 1 },
					{ Feature.InitialPowerBowl1, 4 },
					{ Feature.InitialPowerBowl2, 4 },
					{ Feature.InitialPowerBowl3, 0 },
					{ Feature.BaseCreditsIncome, 0 },
					{ Feature.BaseOresIncome, 1 },
					{ Feature.BaseKnowledgeIncome, 1 },
					{ Feature.BaseQicIncome, 0 },
					{ Feature.BasePowerIncome, 0 },
					{ Feature.BasePowerTokensIncome, 1 },
				}
			},
		};

		#endregion
	}
}