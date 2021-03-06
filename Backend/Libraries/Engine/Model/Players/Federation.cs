using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Model.Board;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class Federation
	{
		public string Id { get; set; }
		public List<string> HexIds { get; set; }
		public int TotalPowerValue { get; set; }
		public int NumBuildings { get; set; }

		public void Add(string playerId, Hex hex)
		{
			if (HexIds.Contains(hex.Id))
			{
				return;
			}
			HexIds.Add(hex.Id);
			var building = hex.Buildings.Single(b => b.PlayerId == playerId);
			TotalPowerValue += building.PowerValueInFederation;
			NumBuildings += building.Type == BuildingType.Satellite || building.Type == BuildingType.IvitsSpaceStation
				? 0
				: 1;
		}

		public void Enlarge(string playerId, List<Hex> additionalHexes)
		{
			var newHexes = additionalHexes.Where(h => !HexIds.Contains(h.Id)).ToList();
			HexIds.AddRange(newHexes.Select(h => h.Id));
			TotalPowerValue += newHexes
				.Select(h => h.Buildings.Single(b => b.PlayerId == playerId).PowerValueInFederation)
				.Sum();
			NumBuildings += newHexes
				.WithBuildings()
				.Except(newHexes.WithSatellites())
				.Except(newHexes.WithIvitsSpaceStation())
				.Count();
		}

		public Federation Clone()
		{
			return new Federation
			{
				Id = Id,
				HexIds = HexIds.ToList(),
				TotalPowerValue = TotalPowerValue,
				NumBuildings = NumBuildings
			};
		}

		public static Federation FromBuildings(string playerId, int counter, List<Hex> hexes)
		{
			return new Federation
			{
				Id = $"{playerId}_{counter}",
				HexIds = hexes.Select(h => h.Id).ToList(),
				TotalPowerValue = hexes
					.SelectMany(h => h.Buildings)
					.Where(b => b.PlayerId == playerId)
					.Select(b => b.PowerValueInFederation)
					.Sum(),
				NumBuildings = hexes
					.WithBuildings()
					.Except(hexes.WithSatellites())
					.Except(hexes.WithIvitsSpaceStation())
					.Count()
			};
		}
	}
}