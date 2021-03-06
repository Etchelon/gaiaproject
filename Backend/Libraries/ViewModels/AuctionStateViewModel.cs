using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels
{
	public class AuctionStateViewModel
	{
		public class AuctionViewModel
		{
			public Race Race { get; set; }
			public int Order { get; set; }
			public string PlayerUsername { get; set; }
			public int? Points { get; set; }
		}

		public List<AuctionViewModel> AuctionedRaces { get; set; }
	}
}