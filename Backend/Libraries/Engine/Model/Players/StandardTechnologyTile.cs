using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class StandardTechnologyTile
	{
		public StandardTechnologyTileType Id { get; set; }
		public bool CoveredByAdvancedTile { get; set; }
		public bool Used { get; set; }

		[BsonIgnore]
		public bool HasAction => Id == StandardTechnologyTileType.ActionGain4Power;

		public StandardTechnologyTile Clone()
		{
			return new StandardTechnologyTile
			{
				Id = Id,
				CoveredByAdvancedTile = CoveredByAdvancedTile,
				Used = Used
			};
		}
	}
}