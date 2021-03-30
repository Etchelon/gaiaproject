using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Setup
{
	[BsonNoId]
	public class SetupPhase
	{
		public SetupSubPhase SubPhase { get; set; } = SetupSubPhase.RandomizeResearchBoard;

		[BsonIgnoreIfNull]
		public AuctionState AuctionState { get; set; }

		public SetupPhase Clone()
		{
			return new SetupPhase
			{
				SubPhase = SubPhase,
				AuctionState = AuctionState.Clone()
			};
		}
	}
}