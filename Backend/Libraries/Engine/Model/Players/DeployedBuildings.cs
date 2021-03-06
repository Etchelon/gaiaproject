using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class DeployedBuildings
	{
		public int Mines { get; set; }
		public int TradingStations { get; set; }
		public int ResearchLabs { get; set; }
		public bool PlanetaryInstitute { get; set; }
		public bool AcademyLeft { get; set; }
		public bool AcademyRight { get; set; }
		public int Satellites { get; set; }
		public int IvitsSpaceStations { get; set; }
		public bool HasLostPlanet { get; set; }

		internal static DeployedBuildings None()
		{
			return new DeployedBuildings();
		}

		public DeployedBuildings Clone()
		{
			return new DeployedBuildings
			{
				Mines = Mines,
				TradingStations = TradingStations,
				ResearchLabs = ResearchLabs,
				PlanetaryInstitute = PlanetaryInstitute,
				AcademyLeft = AcademyLeft,
				AcademyRight = AcademyRight,
				Satellites = Satellites,
				IvitsSpaceStations = IvitsSpaceStations,
				HasLostPlanet = HasLostPlanet
			};
		}
	}
}