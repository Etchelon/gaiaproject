using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Board
{
	public class BoardRandomizer
	{
		private readonly string[] _playerIds;
		private readonly int _nPlayers;
		private readonly MapShape _mapShape;
		private const int NumberOfRounds = 6;
		private const int NumberOfFinalScoringTiles = 2;
		private const int NumberOfFreeStandardTechnologyTiles = 3;
		private const int NumberOfFederationTokensPerType = 3;

		public BoardState Board { get; private set; }

		public BoardRandomizer(IEnumerable<string> playerIds, bool useCustomMap = false)
		{
			_playerIds = playerIds.ToArray();
			_nPlayers = _playerIds.Length;
			_mapShape = useCustomMap ? MapShape.Custom : (MapShape)playerIds.Count();
		}

		public void Randomize()
		{
			Board = new BoardState();
			RandomizeMap();
			RandomizeRounds();
			RandomizeResearchBoard();
			RandomizeRoundBoosters();
			RandomizeFederations();
		}

		private void RandomizeMap()
		{
			var mapService = new MapService(_nPlayers, _mapShape);
			Board.Map = mapService.Map;
		}

		private void RandomizeRounds()
		{
			var allScoringTiles = Enum.GetValues(typeof(RoundScoringTileType)).Cast<int>().ToList();
			allScoringTiles.Shuffle();
			var scoringTiles = allScoringTiles.Take(NumberOfRounds);
			int roundNo = 0;

			var allFinalScoringTiles = Enum.GetValues(typeof(FinalScoringTileType)).Cast<int>().ToList();
			allFinalScoringTiles.Shuffle();
			var finalScoringTiles = allFinalScoringTiles.Take(NumberOfFinalScoringTiles).ToList();

			var random = new Random();
			Board.ScoringBoard = new ScoringBoard
			{
				ScoringTiles = scoringTiles.Select(scoringTile => new ScoringTile
				{
					Id = (RoundScoringTileType)scoringTile,
					RoundNumber = ++roundNo
				})
				.ToArray(),
				FinalScoring1 = new FinalScoringState
				{
					Id = (FinalScoringTileType)finalScoringTiles[0],
					Players = _playerIds.Select(playerId => new FinalScoringState.PlayerState { Count = 0, PlayerId = playerId }).ToList()
				},
				FinalScoring2 = new FinalScoringState
				{
					Id = (FinalScoringTileType)finalScoringTiles[1],
					Players = _playerIds.Select(playerId => new FinalScoringState.PlayerState { Count = 0, PlayerId = playerId }).ToList()
				}
			};
		}

		private void RandomizeResearchBoard()
		{
			var allStandardTechnologyTiles = Enum.GetValues(typeof(StandardTechnologyTileType)).Cast<int>().ToList();
			allStandardTechnologyTiles.Shuffle();

			var allAdvancedTechnologyTiles = Enum.GetValues(typeof(AdvancedTechnologyTileType)).Cast<int>().ToList();
			allAdvancedTechnologyTiles.Shuffle();

			var researchTrackTypes = Enum.GetValues(typeof(ResearchTrackType)).Cast<int>().ToList();

			var tracks = Enumerable.Range(0, researchTrackTypes.Count).Select(n => new ResearchTrack
			{
				Id = (ResearchTrackType)n,
				AdvancedTileType = (AdvancedTechnologyTileType)allAdvancedTechnologyTiles[n],
				IsAdvancedTileAvailable = true,
				StandardTiles = new TechnologyTilePile { Type = (StandardTechnologyTileType)allStandardTechnologyTiles[n], Total = _nPlayers, Remaining = _nPlayers },
				IsLostPlanetAvailable = (ResearchTrackType)n == ResearchTrackType.Navigation
			}).ToArray();
			Board.ResearchBoard = new ResearchBoard
			{
				Tracks = tracks,
				FreeStandardTiles = allStandardTechnologyTiles.Skip(tracks.Length).Take(NumberOfFreeStandardTechnologyTiles).Select(standardTile => new TechnologyTilePile
				{
					Type = (StandardTechnologyTileType)standardTile,
					Total = _nPlayers,
					Remaining = _nPlayers,
				}).ToArray(),
				PowerActions = Enum.GetValues(typeof(PowerActionType)).Cast<int>().Select(type => new PowerActionSpace { Type = (PowerActionType)type, IsAvailable = true }).ToArray(),
				QicActions = Enum.GetValues(typeof(QicActionType)).Cast<int>().Select(type => new QicActionSpace { Type = (QicActionType)type, IsAvailable = true }).ToArray()
			};
		}

		private void RandomizeRoundBoosters()
		{
			var allRoundBoosters = Enum.GetValues(typeof(RoundBoosterType)).Cast<int>().ToList();
			allRoundBoosters.Shuffle();
			var numberOfRoundBoosters = _nPlayers + 3;
			Board.RoundBoosters = new RoundBoosters
			{
				AvailableRoundBooster = allRoundBoosters.Take(numberOfRoundBoosters).Select(roundBooster => new RoundBoosters.RoundBoosterTile
				{
					Id = (RoundBoosterType)roundBooster,
					PlayerId = null
				})
				.ToArray()
			};
		}

		private void RandomizeFederations()
		{
			var allFederationTokens = Enum.GetValues(typeof(FederationTokenType))
				.Cast<int>()
				.Where(n => n != (int)FederationTokenType.Gleens)
				.ToList();
			allFederationTokens.Shuffle();
			var tokenForTerraformingTrack = allFederationTokens.First();
			int tokenNo = 0;
			var track = Board.ResearchBoard.Tracks.First(t => t.Id == ResearchTrackType.Terraformation);
			track.Federation = (FederationTokenType)tokenForTerraformingTrack;
			track.IsFederationTokenAvailable = true;
			Board.Federations = new Federations
			{
				Tokens = allFederationTokens.Select(tokenType =>
				{
					var qty = (NumberOfFederationTokensPerType - (tokenNo++ == 0 ? 1 : 0));
					return new Federations.FederationTokenPile
					{
						Type = (FederationTokenType)tokenType,
						InitialQuantity = qty,
						Remaining = qty
					};
				})
				.ToList()
			};
		}
	}
}
