using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class Map
	{
		public MapShape Shape { get; set; }
		public int ActualPlayerCount { get; set; }
		public List<Hex> Hexes { get; set; }

		public Map Clone()
		{
			return new Map
			{
				Shape = Shape,
				ActualPlayerCount = ActualPlayerCount,
				Hexes = Hexes.Select(h => h.Clone()).ToList()
			};
		}
	}
}
