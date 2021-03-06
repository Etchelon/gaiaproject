using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Setup
{
	[BsonNoId]
	public class AuctionState
	{
		public List<Race> AvailableRaces { get; set; }
		public List<AuctionedRace> Auctions { get; set; }

		public static AuctionState FromAvailableRaces(IEnumerable<Race> availableRaces)
		{
			return new AuctionState
			{
				AvailableRaces = availableRaces.ToList(),
				Auctions = new List<AuctionedRace>(),
			};
		}

		public static AuctionState InitBeforeRaceSelection()
		{
			return new AuctionState
			{
				AvailableRaces = new List<Race>(),
				Auctions = new List<AuctionedRace>(),
			};
		}

		public AuctionState Clone()
		{
			return new AuctionState
			{
				AvailableRaces = AvailableRaces.ToList(),
				Auctions = Auctions.Select(o => o.Clone()).ToList(),
			};
		}
	}
}