using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;
using GaiaProject.ViewModels.Board;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class ResearchAdvancementsResolver : IValueResolver<ResearchTrack, ResearchTrackViewModel, List<PlayerAdvancementViewModel>>
	{
		public List<PlayerAdvancementViewModel> Resolve(ResearchTrack source, ResearchTrackViewModel destination, List<PlayerAdvancementViewModel> destMember, ResolutionContext context)
		{
			var game = context.Items["Game"] as GaiaProjectGame;
			if (game == null)
			{
				throw new ArgumentException($"Game must be passed to PlayerInGameResolver");
			}
			var trackId = source.Id;
			return game.Players
				.Where(p => p.State != null)
				.SelectMany(p => p.State.ResearchAdvancements
					.Where(adv => adv.Track == trackId)
					// If races haven't been picked yet, display nothing
					.Select(adv => p.RaceId.HasValue
						? new { Race = p.RaceId.Value, adv.Steps }
						: null
					)
				)
				.Where(o => o != null)
				.Select(o => new PlayerAdvancementViewModel
				{
					RaceId = o.Race,
					Steps = o.Steps
				})
				.ToList();
		}
	}
}
