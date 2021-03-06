using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Setup
{
	[BsonNoId]
	public class AuctionedRace
	{
		public Race Race { get; set; }
		public string PlayerId { get; set; }
		public int Bid { get; set; } = 0;

		public AuctionedRace Clone()
		{
			return new AuctionedRace
			{
				Race = Race,
				PlayerId = PlayerId,
				Bid = Bid,
			};
		}
	}
}