using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Model.Setup;
using GaiaProject.ViewModels;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class AuctionedRacesResolver : IValueResolver<AuctionState, AuctionStateViewModel, List<AuctionStateViewModel.AuctionViewModel>>
	{
		private readonly UserManager _userManager;

		public AuctionedRacesResolver(UserManager userManager)
		{
			_userManager = userManager;
		}

		public List<AuctionStateViewModel.AuctionViewModel> Resolve(AuctionState auctionState, AuctionStateViewModel destination, List<AuctionStateViewModel.AuctionViewModel> destMember, ResolutionContext context)
		{
			var order = 0;
			var ret = auctionState.AvailableRaces
				.Select(race =>
				{
					var auction = auctionState.Auctions.SingleOrDefault(o => o.Race == race);
					return new AuctionStateViewModel.AuctionViewModel
					{
						Order = order++,
						Race = race,
						PlayerUsername = auction?.PlayerId != null
							? _userManager.GetUsername(auction.PlayerId).Result
							: null,
						Points = auction?.Bid
					};
				})
				.ToList();
			return ret;
		}
	}
}
