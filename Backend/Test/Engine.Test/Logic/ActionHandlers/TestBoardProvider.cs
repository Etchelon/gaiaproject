using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model.Board;

namespace Engine.Test.Logic.ActionHandlers
{
	public class TestBoardProvider
	{
		private readonly string[] _playerUsernames;
		private readonly int _nPlayers;
		private readonly MapShape _mapShape;
		private const int NumberOfRounds = 6;
		private const int NumberOfFinalScoringTiles = 2;
		private const int NumberOfFreeStandardTechnologyTiles = 3;
		private const int NumberOfFederationTokensPerType = 3;
		private const int Seed = 42;

		public BoardState Board { get; private set; }

		public TestBoardProvider(IEnumerable<string> playerUsernames, bool useCustomMap = false)
		{
			_playerUsernames = playerUsernames.ToArray();
			_nPlayers = _playerUsernames.Length;
			_mapShape = useCustomMap ? MapShape.Custom : (MapShape)playerUsernames.Count();
			InitializeState();
		}

		private void InitializeState()
		{
			Board = new BoardState();
			SetupMap();
			SetupRounds();
			SetupResearchBoard();
			SetupRoundBoosters();
			SetupFederations();
		}

		private void SetupMap()
		{
			var is2player = _nPlayers <= 2;
			var mapService = new MapService(_nPlayers, is2player ? MapShape.IntroductoryGame2P : MapShape.IntroductoryGame34P);
			Board.Map = mapService.Map;
		}

		private void SetupRounds()
		{
			var allScoringTiles = Enum.GetValues(typeof(RoundScoringTileType)).Cast<int>().ToList();
			var scoringTiles = allScoringTiles.Take(NumberOfRounds);
			int roundNo = 0;

			var allFinalScoringTiles = Enum.GetValues(typeof(FinalScoringTileType)).Cast<int>().ToList();
			var finalScoringTiles = allFinalScoringTiles.Take(NumberOfFinalScoringTiles).ToList();

			var random = new Random(Seed);
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
					Players = _playerUsernames.Select(playerId => new FinalScoringState.PlayerState { Count = random.Next(14), PlayerId = playerId }).ToList()
				},
				FinalScoring2 = new FinalScoringState
				{
					Id = (FinalScoringTileType)finalScoringTiles[1],
					Players = _playerUsernames.Select(playerId => new FinalScoringState.PlayerState { Count = random.Next(14), PlayerId = playerId }).ToList()
				}
			};
		}

		private void SetupResearchBoard()
		{
			var allStandardTechnologyTiles = Enum.GetValues(typeof(StandardTechnologyTileType)).Cast<int>().ToList();
			var allAdvancedTechnologyTiles = Enum.GetValues(typeof(AdvancedTechnologyTileType)).Cast<int>().ToList();
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

		private void SetupRoundBoosters()
		{
			var allRoundBoosters = Enum.GetValues(typeof(RoundBoosterType)).Cast<int>().ToList();
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

		private void SetupFederations()
		{
			var allFederationTokens = Enum.GetValues(typeof(FederationTokenType))
				.Cast<int>()
				.Where(n => n != (int)FederationTokenType.Gleens)
				.ToList();
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
