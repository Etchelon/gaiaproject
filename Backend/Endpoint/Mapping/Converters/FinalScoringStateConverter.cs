using System;
using System.Linq;
using AutoMapper;
using GaiaProject.ViewModels.Board;
using GaiaProject.Engine.Model.Board;
using GaiaProject.ViewModels.Players;
using GaiaProject.Engine.Model;

namespace GaiaProject.Endpoint.Mapping.Converters
{
	public class FinalScoringStateConverter : ITypeConverter<FinalScoringState.PlayerState, FinalScoringStateViewModel.PlayerFinalScoringStatusViewModel>
	{
		public FinalScoringStateViewModel.PlayerFinalScoringStatusViewModel Convert(FinalScoringState.PlayerState source, FinalScoringStateViewModel.PlayerFinalScoringStatusViewModel destination, ResolutionContext context)
		{
			var game = context.Items["Game"] as GaiaProjectGame;
			if (game == null)
			{
				throw new ArgumentException($"Game must be passed to FinalScoringStateConverter");
			}
			var player = game.Players.First(p => p.Id == source.PlayerId);
			return new FinalScoringStateViewModel.PlayerFinalScoringStatusViewModel
			{
				Player = context.Mapper.Map<PlayerInfoViewModel>(player),
				Count = source.Count,
				Points = source.Points,
			};
		}
	}
}