using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class RoundBooster
	{
		public RoundBoosterType Id { get; set; }
		public bool Used { get; set; }

		[BsonIgnore]
		public bool HasAction => Id == RoundBoosterType.BoostRangeGainPower
								 || Id == RoundBoosterType.TerraformActionGainCredits;

		public RoundBooster Clone()
		{
			return new RoundBooster
			{
				Id = Id,
				Used = Used
			};
		}
	}
}