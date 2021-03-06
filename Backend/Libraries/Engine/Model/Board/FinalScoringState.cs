using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class FinalScoringState
	{
		[BsonNoId]
		public class PlayerState
		{
			public string PlayerId { get; set; }
			public int Count { get; set; }
			public int Points { get; set; }
		}

		public FinalScoringTileType Id { get; set; }
		public List<PlayerState> Players { get; set; }
	}
}
