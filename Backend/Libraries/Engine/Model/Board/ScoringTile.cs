using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class ScoringTile
	{
		public RoundScoringTileType Id { get; set; }
		public int RoundNumber { get; set; }
	}
}
