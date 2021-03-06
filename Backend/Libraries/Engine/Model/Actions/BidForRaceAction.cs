using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(BidForRaceAction))]
	public class BidForRaceAction : PlayerAction
	{
		public override ActionType Type => ActionType.BidForRace;
		public Race Race { get; set; }
		public int Points { get; set; }

		public override string ToString()
		{
			return $"{PlayerUsername} bids {Points} for {Race.ToDescription()}";
		}
	}
}