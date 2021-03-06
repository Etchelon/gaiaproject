using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class PowerPools
	{
		public enum BrainstoneLocation
		{
			Removed,
			Bowl1,
			Bowl2,
			Bowl3,
			GaiaArea
		}

		public int Bowl1 { get; set; }
		public int Bowl2 { get; set; }
		public int Bowl3 { get; set; }
		public int GaiaArea { get; set; }
		public BrainstoneLocation? Brainstone { get; set; }

		[BsonIgnore]
		public bool Empty => (Bowl1 + Bowl2 + Bowl3) == 0 && (!Brainstone.HasValue || (Brainstone == BrainstoneLocation.Removed || Brainstone == BrainstoneLocation.GaiaArea));

		public PowerPools Clone()
		{
			return new PowerPools
			{
				Bowl1 = Bowl1,
				Bowl2 = Bowl2,
				Bowl3 = Bowl3,
				GaiaArea = GaiaArea,
				Brainstone = Brainstone
			};
		}
	}
}