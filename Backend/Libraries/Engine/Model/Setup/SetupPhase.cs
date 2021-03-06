using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Reflection;

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
			return ReflectionUtils.Clone(this);
		}
	}
}