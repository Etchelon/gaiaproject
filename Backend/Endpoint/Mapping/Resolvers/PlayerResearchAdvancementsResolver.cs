using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Players;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class PlayerResearchAdvancementsResolver : IValueResolver<PlayerState, PlayerStateViewModel, ResearchAdvancementsViewModel>
	{
		public ResearchAdvancementsViewModel Resolve(PlayerState source, PlayerStateViewModel destination, ResearchAdvancementsViewModel destMember, ResolutionContext context)
		{
			return new ResearchAdvancementsViewModel
			{
				Terraformation = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.Terraformation).Steps,
				Navigation = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.Navigation).Steps,
				ArtificialIntelligence = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.ArtificialIntelligence).Steps,
				Gaiaformation = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.Gaiaformation).Steps,
				Economy = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.Economy).Steps,
				Science = source.ResearchAdvancements.First(rad => rad.Track == ResearchTrackType.Science).Steps
			};
		}
	}
}
