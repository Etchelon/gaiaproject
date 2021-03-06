using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class Map
	{
		public MapShape Shape { get; set; }
		public int ActualPlayerCount { get; set; }
		public Hex[] Hexes { get; set; }
	}
}
