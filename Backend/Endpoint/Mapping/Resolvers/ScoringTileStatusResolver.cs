using System;
using AutoMapper;
using GaiaProject.ViewModels.Board;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class ScoringTileStatusResolver : IValueResolver<ScoringTile, ScoringTileViewModel, bool>
	{
		public bool Resolve(ScoringTile tile, ScoringTileViewModel tileViewModel, bool inactive, ResolutionContext context)
		{
			var game = context.Items["Game"] as GaiaProjectGame;
			if (game == null)
			{
				throw new ArgumentException($"Game must be passed to ScoringTileStatusResolver");
			}
			if (game.CurrentPhaseId == GamePhase.Setup)
			{
				return false;
			}
			var currentRound = game.Rounds.CurrentRound;
			return currentRound > tile.RoundNumber;
		}
	}
}
