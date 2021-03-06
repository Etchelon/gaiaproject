using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class PointUtils
	{
		public const string GhostPlayer = "GhostPlayer";

		public static List<PointsGain> GetPointsForBuildingStructure(BuildingType type, string playerId, PlanetType? planetType, GaiaProjectGame game)
		{
			Debug.Assert(type != BuildingType.Mine || planetType != null, nameof(planetType) + " != null");
			return (type switch
			{
				var t when
					t == BuildingType.PlanetaryInstitute
					|| t == BuildingType.AcademyLeft
					|| t == BuildingType.AcademyRight
					=> new List<PointsGain> { GetPointsForBigBuilding(game) },
				BuildingType.Mine => GetPointsForBuildingMine(playerId, planetType.Value, game),
				BuildingType.TradingStation => GetPointsForBuildingTradingStation(playerId, game),
				_ => new List<PointsGain>()
			}).Where(g => g != null).ToList();
		}

		private static List<PointsGain> GetPointsForBuildingMine(string playerId, PlanetType planetType, GaiaProjectGame game)
		{
			var ret = new List<PointsGain>();
			// Check if player has advanced tile with points per mine
			var player = game.GetPlayer(playerId);
			var hasAdvancedTile = player.State.AdvancedTechnologyTiles.Any(tt => tt.Id == AdvancedTechnologyTileType.Passive3PointsPerMine);
			if (hasAdvancedTile)
			{
				ret.Add(new PointsGain(3, "Advanced tile"));
			}
			// Now check if it was a Gaia planet or not
			// Assume that we calculate points after a successful build,
			// therefore a Transdim planet must have been Gaiaformed.
			var currentRound = game.Rounds.CurrentRound;
			var currentRoundTile = game.BoardState.ScoringBoard.ScoringTiles.Single(st => st.RoundNumber == currentRound);
			if (currentRoundTile.Id == RoundScoringTileType.PointsPerMine2)
			{
				ret.Add(new PointsGain(2, "Scoring tile"));
			}

			var isGaiaPlanet = planetType == PlanetType.Gaia || planetType == PlanetType.Transdim;
			if (isGaiaPlanet)
			{
				var hasBaseTile = player.State.StandardTechnologyTiles.Any(tt => tt.Id == StandardTechnologyTileType.Passive3PointsPerGaiaPlanet);
				if (hasBaseTile)
				{
					ret.Add(new PointsGain(3, $"Standard tile"));
				}
				ret.Add(currentRoundTile.Id switch
				{
					RoundScoringTileType.PointsPerGaiaPlanet4 => new PointsGain(4, "Scoring tile"),
					RoundScoringTileType.PointsPerGaiaPlanet3 => new PointsGain(3, "Scoring tile"),
					_ => null
				});
				if (player.RaceId == Race.Gleens)
				{
					ret.Add(new PointsGain(2, "Gleens's ability"));
				}
			}
			return ret;
		}

		private static List<PointsGain> GetPointsForBuildingTradingStation(string playerId, GaiaProjectGame game)
		{
			var ret = new List<PointsGain>();
			// Check if player has advanced tile with points per mine
			var player = game.GetPlayer(playerId);
			var hasAdvancedTile = player.State.AdvancedTechnologyTiles.Any(tt => tt.Id == AdvancedTechnologyTileType.Passive3PointsPerTradingStation);
			if (hasAdvancedTile)
			{
				ret.Add(new PointsGain(3, "Advanced tile"));
			}
			var currentRound = game.Rounds.CurrentRound;
			var currentRoundTile = game.BoardState.ScoringBoard.ScoringTiles.Single(st => st.RoundNumber == currentRound);
			ret.Add(currentRoundTile.Id switch
			{
				RoundScoringTileType.PointsPerTradingStation4 => new PointsGain(4, "Scoring tile"),
				RoundScoringTileType.PointsPerTradingStation3 => new PointsGain(3, "Scoring tile"),
				_ => null
			});
			return ret;
		}

		private static PointsGain GetPointsForBigBuilding(GaiaProjectGame game)
		{
			// Check if player has advanced tile with points per mine
			var currentRound = game.Rounds.CurrentRound;
			var currentRoundTile = game.BoardState.ScoringBoard.ScoringTiles.Single(st => st.RoundNumber == currentRound);
			return currentRoundTile.Id switch
			{
				RoundScoringTileType.PointsPerBigBuilding5 => new PointsGain(5, "Scoring tile"),
				RoundScoringTileType.PointsPerBigBuilding5Bis => new PointsGain(5, "Scoring tile"),
				_ => null
			};
		}

		public static PointsGain GetPointsForLastGaiaformationStep(string playerId, GaiaProjectGame game)
		{
			var map = game.BoardState.Map;
			var player = game.Players.First(p => p.Id == playerId);
			return new PointsGain(4 + player.State.GaiaPlanets, "Gaiaformation technology level 5");
		}

		public static List<PointsGain> GetPointsUponPassing(string playerId, GaiaProjectGame game)
		{
			var ret = new List<PointsGain>();
			var player = game.GetPlayer(playerId);
			var booster = player.State.RoundBooster.Id;
			var fromBooster = GetPointsForBooster(booster, playerId, game);
			if (fromBooster > 0)
			{
				ret.Add(new PointsGain(fromBooster, "Round booster"));
			}

			var fromAdvancedTiles = player.State.AdvancedTechnologyTiles
				.Select(t => t.Id switch
				{
					AdvancedTechnologyTileType.Pass1PointsPerPlanetType => new PointsGain(player.State.KnownPlanetTypes.Count, "Advanced tile"),
					AdvancedTechnologyTileType.Pass3PointsPerFederation => new PointsGain(player.State.FederationTokens.Count * 3, "Advanced tile"),
					AdvancedTechnologyTileType.Pass3PointsPerResearchLab => new PointsGain(player.State.Buildings.ResearchLabs * 3, "Advanced tile"),
					_ => null
				})
				.ToList();
			ret.AddRange(fromAdvancedTiles.Where(gain => (gain?.Points ?? 0) > 0));

			return ret;
		}

		private static int GetPointsForBooster(RoundBoosterType booster, string playerId, GaiaProjectGame game)
		{
			var player = game.Players.First(p => p.Id == playerId);
			var playerBuildings = player.State.Buildings;
			return booster switch
			{
				RoundBoosterType.PassPointsPerMineGainOre => playerBuildings.Mines + (playerBuildings.HasLostPlanet ? 1 : 0),
				RoundBoosterType.PassPointsPerTradingStationsGainOre => (2 * playerBuildings.TradingStations),
				RoundBoosterType.PassPointsPerResearchLabsGainKnowledge => (3 * playerBuildings.ResearchLabs),
				RoundBoosterType.PassPointsPerBigBuildingsGainPower => (
					4 * (0 + (playerBuildings.PlanetaryInstitute ? 1 : 0) + (playerBuildings.AcademyLeft ? 1 : 0) +
						 (playerBuildings.AcademyRight ? 1 : 0))),
				RoundBoosterType.PassPointsPerGaiaPlanetsGainCredits => player.State.GaiaPlanets,
				_ => 0
			};
		}

		public static List<PointsGain> GetPointsForResearchStep(string playerId, GaiaProjectGame game)
		{
			var ret = new List<PointsGain>();

			var roundNumber = game.Rounds.CurrentRound;
			var roundTile = game.BoardState.ScoringBoard.ScoringTiles.Single(st => st.RoundNumber == roundNumber);
			var currentRoundGivesPoints = roundTile.Id == RoundScoringTileType.PointsPerResearchStep2;
			if (currentRoundGivesPoints)
			{
				ret.Add(new PointsGain(2, "Scoring Tile"));
			}

			var player = game.GetPlayer(playerId);
			var hasAdvancedTile = player.State.AdvancedTechnologyTiles.Any(at =>
				at.Id == AdvancedTechnologyTileType.Passive2PointsPerResearchStep);
			if (hasAdvancedTile)
			{
				ret.Add(new PointsGain(2, "Advanced Tile"));
			}

			return ret;
		}

		public static (List<FinalScoringState.PlayerState> finalScoring1, List<FinalScoringState.PlayerState> finalScoring2) ProcessFinalScorings(GaiaProjectGame game)
		{
			var firstScoringPlayers = ProcessFinalScoring(game, game.BoardState.ScoringBoard.FinalScoring1);
			var secondScoringPlayers = ProcessFinalScoring(game, game.BoardState.ScoringBoard.FinalScoring2);
			return (firstScoringPlayers, secondScoringPlayers);
		}

		private static int GetGhostPlayerCount(FinalScoringTileType tile)
		{
			return tile switch
			{
				FinalScoringTileType.BuildingsInAFederation => 10,
				FinalScoringTileType.BuildingsOnTheMap => 11,
				FinalScoringTileType.GaiaPlanets => 4,
				FinalScoringTileType.KnownPlanetTypes => 5,
				FinalScoringTileType.Satellites => 8,
				FinalScoringTileType.Sectors => 6,
				_ => throw new ArgumentOutOfRangeException("Final Scoring type")
			};
		}

		private static int CountElements(FinalScoringTileType type, PlayerInGame player)
		{
			return type switch
			{
				FinalScoringTileType.BuildingsInAFederation =>
					player.State.Federations.Select(fed => fed.NumBuildings).Sum(),
				FinalScoringTileType.BuildingsOnTheMap =>
					player.State.Buildings.Mines +
					player.State.Buildings.TradingStations +
					player.State.Buildings.ResearchLabs +
					(player.State.Buildings.PlanetaryInstitute ? 1 : 0) +
					(player.State.Buildings.AcademyLeft ? 1 : 0) +
					(player.State.Buildings.AcademyRight ? 1 : 0) +
					(player.State.Buildings.HasLostPlanet ? 1 : 0),
				FinalScoringTileType.GaiaPlanets => player.State.GaiaPlanets,
				FinalScoringTileType.KnownPlanetTypes => player.State.KnownPlanetTypes.Count,
				FinalScoringTileType.Satellites => player.State.Buildings.Satellites + player.State.Buildings.IvitsSpaceStations,
				FinalScoringTileType.Sectors => player.State.ColonizedSectors.Count,
				_ => throw new ArgumentOutOfRangeException("Final scoring type")
			};
		}

		private static List<FinalScoringState.PlayerState> ProcessFinalScoring(GaiaProjectGame game, FinalScoringState finalScoring)
		{
			var scoredPlayers = game.Players;
			var scoringType = finalScoring.Id;
			var playersAndCounts = scoredPlayers
				.Select(p => p.State != null
					? new { PlayerId = p.Id, Count = CountElements(scoringType, p) }
					: new { PlayerId = p.Id, Count = 0 }
				)
				.ToList();
			if (scoredPlayers.Count < 4)
			{
				playersAndCounts.Add(new { PlayerId = GhostPlayer, Count = GetGhostPlayerCount(scoringType) });
			}
			var sortedPlayers = playersAndCounts.OrderByDescending(o => o.Count).ToList();
			var points = new List<int> { 18, 12, 6, 0 };
			var players = new List<FinalScoringState.PlayerState>();
			sortedPlayers.ForEach(o =>
			{
				if (players.Any(p => p.PlayerId == o.PlayerId))
				{
					return;
				}
				var tiedPlayers = sortedPlayers.Where(p => p.Count == o.Count);
				var tiedPlayersCount = tiedPlayers.Count();
				var pointsToDistribute = points.GetRange(0, tiedPlayersCount).Sum();
				var pointsForEach = (int)Math.Floor((double)pointsToDistribute / tiedPlayersCount);
				players.AddRange(tiedPlayers.Select(tp => new FinalScoringState.PlayerState
				{
					PlayerId = tp.PlayerId,
					Count = tp.Count,
					Points = pointsForEach
				}));
				points = points.GetRange(tiedPlayersCount, points.Count - tiedPlayersCount);
			});
			return players;
		}

		public static void CalculatePlayersPlacement(IEnumerable<PlayerInGame> players_)
		{
			var position = 1;
			players_
				.GroupBy(p => p.State.Points)
				.OrderByDescending(g => g.Key)
				.ToList()
				.ForEach(g =>
				{
					g.ToList().ForEach(p => p.Placement = position);
					position += g.Count();
				});
		}
	}
}