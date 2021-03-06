using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class TechnologyTilePile
	{
		public StandardTechnologyTileType Type { get; set; }
		public int Total { get; set; }
		public int Remaining { get; set; }
	}
}
