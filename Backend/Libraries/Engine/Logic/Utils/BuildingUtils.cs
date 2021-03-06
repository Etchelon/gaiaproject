using System;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class BuildingUtils
	{
		public const int NumMines = 8;
		private const int NumTradingStations = 4;
		private const int NumResearchLabs = 3;
		private const int NumSatellites = 25;
		private const int NumIvitsSpaceStations = 6;

		public static bool HasSpareBuildingsOfType(BuildingType type, PlayerInGame player)
		{
			var playersBuildings = player.State.Buildings;
			return type switch
			{
				BuildingType.Mine => playersBuildings.Mines < NumMines,
				BuildingType.TradingStation => playersBuildings.TradingStations < NumTradingStations,
				BuildingType.ResearchLab => playersBuildings.ResearchLabs < NumResearchLabs,
				BuildingType.PlanetaryInstitute => !playersBuildings.PlanetaryInstitute,
				BuildingType.AcademyLeft => !playersBuildings.AcademyLeft,
				BuildingType.AcademyRight => !playersBuildings.AcademyRight,
				BuildingType.Gaiaformer => player.State.Gaiaformers.Any(gf => gf.Unlocked && gf.Available),
				BuildingType.Satellite => playersBuildings.Satellites < NumSatellites,
				BuildingType.LostPlanet => throw new NotImplementedException(
					"Meaningless to check for this type of building here"),
				BuildingType.IvitsSpaceStation => playersBuildings.IvitsSpaceStations < NumIvitsSpaceStations,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		public static bool CanUpgradeTo(BuildingType sourceBuildingType, BuildingType targetBuildingType, Race race)
		{
			return sourceBuildingType switch
			{
				BuildingType.Mine => (targetBuildingType == BuildingType.TradingStation),
				BuildingType.TradingStation => (targetBuildingType == BuildingType.ResearchLab || (race == Race.Bescods
					? targetBuildingType == BuildingType.AcademyLeft ||
					  targetBuildingType == BuildingType.AcademyRight
					: targetBuildingType == BuildingType.PlanetaryInstitute)),
				BuildingType.ResearchLab => (race == Race.Bescods
					? targetBuildingType == BuildingType.PlanetaryInstitute
					: targetBuildingType == BuildingType.AcademyLeft ||
					  targetBuildingType == BuildingType.AcademyRight),
				_ => false
			};
		}
	}
}