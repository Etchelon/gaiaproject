using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class ResearchBoard
	{
		public ResearchTrack[] Tracks { get; set; }
		public TechnologyTilePile[] FreeStandardTiles { get; set; }
		public PowerActionSpace[] PowerActions { get; set; }
		public QicActionSpace[] QicActions { get; set; }
	}
}
