using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class ScoringBoard
	{
		public ScoringTile[] ScoringTiles { get; set; }
		public FinalScoringState FinalScoring1 { get; set; }
		public FinalScoringState FinalScoring2 { get; set; }
	}
}
