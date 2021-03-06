using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class PlayerResources
	{
		public int Credits { get; set; }
		public int Ores { get; set; }
		public int Knowledge { get; set; }
		public int Qic { get; set; }
		public PowerPools Power { get; set; } = new PowerPools();

		public PlayerResources Clone()
		{
			return new PlayerResources
			{
				Credits = Credits,
				Ores = Ores,
				Knowledge = Knowledge,
				Qic = Qic,
				Power = Power.Clone()
			};
		}
	}
}