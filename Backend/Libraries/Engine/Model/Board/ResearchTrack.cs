using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class ResearchTrack
	{
		public ResearchTrackType Id { get; set; }
		public TechnologyTilePile StandardTiles { get; set; }
		public AdvancedTechnologyTileType AdvancedTileType { get; set; }
		public bool IsAdvancedTileAvailable { get; set; }
		public FederationTokenType Federation { get; set; }
		public bool IsFederationTokenAvailable { get; set; }
		public bool IsLostPlanetAvailable { get; set; }
	}
}
