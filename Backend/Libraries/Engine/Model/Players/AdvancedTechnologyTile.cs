using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class AdvancedTechnologyTile
	{
		public AdvancedTechnologyTileType Id { get; set; }
		public StandardTechnologyTileType CoveredTile { get; set; }
		public bool Used { get; set; }

		[BsonIgnore]
		public bool HasAction => Id == AdvancedTechnologyTileType.ActionGain1Qic5Credits
								 || Id == AdvancedTechnologyTileType.ActionGain3Knowledge
								 || Id == AdvancedTechnologyTileType.ActionGain3Ores;

		public AdvancedTechnologyTile Clone()
		{
			return new AdvancedTechnologyTile
			{
				Id = Id,
				CoveredTile = CoveredTile,
				Used = Used
			};
		}
	}
}