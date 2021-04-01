using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class Hex
	{
		public string Id => $"{SectorId}-{Index}";
		public string SectorId { get; set; }
		public int SectorNumber { get; set; }
		public int SectorRotation { get; set; }
		public short Index { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public PlanetType? PlanetType { get; set; }
		public bool? WasGaiaformed { get; set; }
		public List<Building> Buildings { get; set; }

		[BsonIgnore]
		public PlanetType? ActualPlanetType => PlanetType == Enums.PlanetType.Gaia ||
											   PlanetType == Enums.PlanetType.Transdim && (WasGaiaformed ?? false)
			? Enums.PlanetType.Gaia
			: PlanetType;

		public static Hex Create(string sectorId, int sectorNumber, int sectorRotation, short index, int row, int column, PlanetType? planetType)
		{
			return new Hex
			{
				SectorId = sectorId,
				SectorNumber = sectorNumber,
				SectorRotation = sectorRotation,
				Index = index,
				Row = row,
				Column = column,
				PlanetType = planetType,
				Buildings = new List<Building>()
			};
		}

		public Hex Clone()
		{
			return new Hex
			{
				SectorId = SectorId,
				SectorNumber = SectorNumber,
				SectorRotation = SectorRotation,
				Index = Index,
				Row = Row,
				Column = Column,
				PlanetType = PlanetType,
				WasGaiaformed = WasGaiaformed,
				Buildings = Buildings.Select(b => b.Clone()).ToList()
			};
		}
	}
}
