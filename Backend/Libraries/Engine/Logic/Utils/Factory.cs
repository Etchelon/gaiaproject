using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;
using GaiaProject.Engine.Model.Rounds;
using GaiaProject.Engine.Model.Setup;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class Factory
	{
		public static SetupPhase InitialSetupPhase(string initialPlayerId, List<PlayerInGame> players, GameOptions options)
		{
			var nPlayers = players.Count;
			SetupSubPhase subPhase;
			AuctionState auctionState = null;
			var firstPlayer = players.OrderBy(p => p.TurnOrder).First();
			if (options.Auction)
			{
				if (options.FactionSelectionMode == RaceSelectionMode.Random)
				{
					subPhase = SetupSubPhase.Auction;
					var races = RaceUtils.RandomizeRaces(nPlayers);
					auctionState = AuctionState.FromAvailableRaces(races);
					firstPlayer.Actions = ActionState.FromAction(ActionType.BidForRace);
				}
				else
				{
					subPhase = SetupSubPhase.SelectRaces;
					auctionState = AuctionState.InitBeforeRaceSelection();
					firstPlayer.Actions = ActionState.FromAction(ActionType.SelectRace);
				}
			}
			else
			{
				if (options.FactionSelectionMode == RaceSelectionMode.Random)
				{
					subPhase = SetupSubPhase.InitialPlacement;
					var orderedPlayers = players.OrderBy(p => p.InitialOrder).ToArray();
					var isIvits = firstPlayer.RaceId == Race.Ivits;
					if (isIvits)
					{
						firstPlayer = orderedPlayers.Skip(1).First();
					}
					firstPlayer.Actions = ActionState.FromAction(ActionType.PlaceInitialStructure);
				}
				else
				{
					subPhase = SetupSubPhase.SelectRaces;
					var orderedPlayers = players.OrderBy(p => p.InitialOrder);
					firstPlayer.Actions = ActionState.FromAction(ActionType.SelectRace);
				}
			}

			return new SetupPhase
			{
				SubPhase = subPhase,
				AuctionState = auctionState,
			};
		}

		public static RoundsPhase InitialRoundsPhase(GaiaProjectGame game)
		{
			var firstPlayer = game.Players.OrderBy(p => p.TurnOrder).First();
			firstPlayer.Actions.ResetForNewRound().MustTakeMainAction();
			return new RoundsPhase();
		}

		public static PlayerState InitialPlayerState(Race race, int turnOrder, int startingVPs = 10, int? auctionPoints = null)
		{
			return new PlayerState
			{
				CurrentRoundTurnOrder = turnOrder,
				NextRoundTurnOrder = null,
				Points = startingVPs,
				AuctionPoints = auctionPoints,
				Range = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialNavigationRange),
				TerraformingCost = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialTerraformingCost),
				Federations = new List<Federation>(),
				AdvancedTechnologyTiles = new List<AdvancedTechnologyTile>(),
				StandardTechnologyTiles = new List<StandardTechnologyTile>(),
				Resources = InitialResources(race),
				Gaiaformers = Enumerable.Range(0, 3).Select(n => InitialGaiaformerState(n, race)).ToList(),
				RoundBooster = null,
				Incomes = IncomeUtils.GetBaseIncomes(race),
				ResearchAdvancements = GetInitialResearchAdvancements(race),
				Buildings = DeployedBuildings.None()
			};
		}

		private static PlayerResources InitialResources(Race race)
		{
			var initialStep = RaceUtils.GetInitialResearchStep(race);
			var isGleen = race == Race.Gleens;
			var bonusInitialOres = initialStep == ResearchTrackType.Terraformation
				? 2
				// Gleens don't get the Qic from navigation at start, they get an ore instead
				: isGleen
				? 1
				: 0;
			var bonusInitialQic = initialStep == ResearchTrackType.ArtificialIntelligence
				|| (initialStep == ResearchTrackType.Navigation && !isGleen) ? 1 : 0;
			return new PlayerResources
			{
				Credits = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialCredits),
				Ores = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialOres) + bonusInitialOres,
				Knowledge = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialKnowledge),
				Qic = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialQic) + bonusInitialQic,
				Power = new PowerPools
				{
					Bowl1 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl1),
					Bowl2 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl2),
					Bowl3 = RaceUtils.GetRaceInitialLevel(race, RaceUtils.Feature.InitialPowerBowl3),
					GaiaArea = 0,
					Brainstone = race == Race.Taklons ? PowerPools.BrainstoneLocation.Bowl1 : (PowerPools.BrainstoneLocation?)null
				}
			};
		}

		private static List<ResearchAdvancement> GetInitialResearchAdvancements(Race race)
		{
			var initialStep = RaceUtils.GetInitialResearchStep(race);
			var enumValues = typeof(ResearchTrackType)
				.GetEnumValues()
				.Cast<ResearchTrackType>()
				.ToArray();
			var ret = new List<ResearchAdvancement>(enumValues.Length);
			foreach (var val in enumValues)
			{
				var steps = initialStep == val ? 1 : 0;
				ret.Add(new ResearchAdvancement { Track = val, Steps = steps });
			}
			return ret;
		}

		private static Gaiaformer InitialGaiaformerState(int index, Race race)
		{
			var initialStep = RaceUtils.GetInitialResearchStep(race);
			return initialStep == ResearchTrackType.Gaiaformation && index == 0
				? new Gaiaformer
				{
					Id = index,
					Unlocked = true,
					Available = true,
					OnHexId = null,
					SpentInGaiaArea = false
				}
				: new Gaiaformer
				{
					Id = index,
					Unlocked = false,
					Available = false,
					OnHexId = null,
					SpentInGaiaArea = false
				};
		}
	}
}