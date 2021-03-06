using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class RoundBoosters
	{
		[BsonNoId]
		public class RoundBoosterTile
		{
			public RoundBoosterType Id { get; set; }
			public string PlayerId { get; set; }

			[BsonIgnore]
			public bool IsTaken => PlayerId != null;
		}

		public RoundBoosterTile[] AvailableRoundBooster { get; set; }
	}
}
