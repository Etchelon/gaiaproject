using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GaiaProject.Common;
using GaiaProject.Common.Utils;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GaiaProject.Engine.Logic.Board.Map
{
	public class MapService
	{
		#region Data Model

		public class ElementPosition
		{
			public int Row { get; set; }
			public int Column { get; set; }
		}

		public class SectorData
		{
			public class PlanetInSector
			{
				public short HexIndex { get; set; }
				public PlanetType PlanetType { get; set; }
			}

			public string Id { get; set; }
			public List<PlanetInSector> Planets { get; set; }
		}

		public class MapShapeData
		{
			public MapShape ShapeId { get; set; }
			public List<string> UsedSectors { get; set; }
			public List<ElementPosition> SectorPositions { get; set; }
		}

		public class MapData
		{
			public List<ElementPosition> HexPositions { get; set; }
			public List<SectorData> Sectors { get; set; }
			public List<MapShapeData> MapShapes { get; set; }
		}

		#endregion

		public const int RangeBoostFromQic = 2;
		public const int MaxRotationSteps = 6;
		public const short RowsInSector = 9;
		public const short ColumnsInSector = 5;
		private readonly Dictionary<int, List<int>> _rotationOffsetsHalfSequences = new Dictionary<int, List<int>>{
			{ 0, new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
			{ 1, new List<int> { 5, 1, 8, -3, 3, 10, -2, 5, -7 } },
			{ 2, new List<int> { 15, 9, 15, 2, 8, 13, 1, 7, -6 } },
			{ 3, new List<int> { 18, 16, 14, 12, 10, 8, 6, 4, 2 } },
			{ 4, new List<int> { 13, 15, 6, 15, 7, -2, 8, -1, 9 } },
			{ 5, new List<int> { 3, 7, -1, 10, 2, -5, 5, -3, 8 } }
		};

		private readonly int _nPlayers;
		private readonly bool _isIntroductoryGame;
		private readonly MapShapeData _mapShapeData;
		private readonly MapData _mapData;
		public Model.Board.Map Map { get; private set; }

		private MapService()
		{
			var executablePath = Common.Filesystem.Utils.GetExecutingDirectoryName();
			var path = Path.Combine(executablePath, "Logic/Board/Map/MapData.json");
			var mapDataFile = File.ReadAllText(path, Encoding.UTF8);
			var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			_mapData = JsonConvert.DeserializeObject<MapData>(mapDataFile, settings);
		}

		public MapService(int nPlayers, MapShape mapShape) : this()
		{
			_nPlayers = nPlayers;
			_isIntroductoryGame = mapShape == MapShape.IntroductoryGame2P || mapShape == MapShape.IntroductoryGame34P;
			_mapShapeData = _mapData.MapShapes.First(s => s.ShapeId == mapShape);
			CreateMap();
		}

		public MapService(Model.Board.Map map, bool clone = false) : this()
		{
			var nPlayers = map.ActualPlayerCount;
			var mapShape = map.Shape;
			_nPlayers = nPlayers;
			_isIntroductoryGame = mapShape == MapShape.IntroductoryGame2P || mapShape == MapShape.IntroductoryGame34P;
			_mapShapeData = _mapData.MapShapes.First(s => s.ShapeId == mapShape);
			Map = clone ? map.Clone() : map;
		}

		private void CreateMap()
		{
			//var sectorData = _mapData.Sectors.Where(s => _mapShapeData.UsedSectors.Contains(s.Id)).ToList();
			var sectorData = _mapShapeData.UsedSectors
				.Select(sectorId => _mapData.Sectors.First(s => s.Id == sectorId))
				.ToList();
		CREATION:
			if (!_isIntroductoryGame)
			{
				sectorData.Shuffle();
			}
			int sectorNumber = 0;
			var sectorsAndPositions = sectorData.Zip(_mapShapeData.SectorPositions, (sector, position) => new { Sector = sector, Position = position });
			var hexes = sectorsAndPositions.SelectMany(
				o => CreateHexes(o.Sector, o.Position, sectorNumber++, _isIntroductoryGame ? 0 : (byte?)null)
			).ToList();
			Map = new Model.Board.Map
			{
				Shape = _mapShapeData.ShapeId,
				ActualPlayerCount = _nPlayers,
				Hexes = hexes
			};
			var (isValid, invalidHexIds) = FindInvalidHexes();
			var count = 0;
			while (!isValid)
			{
				if (count++ > 10)
				{
					goto CREATION;
				}
				Console.WriteLine($"Map is invalid! Found some adjacent hexes with the same planet. Attempt to fix no. {count}");
				FixInvalidHexes(invalidHexIds);
				(isValid, invalidHexIds) = FindInvalidHexes();
			}
		}

		public (bool isValid, List<string> invalidHexIds) FindInvalidHexes()
		{
			foreach (var hex in Map.Hexes)
			{
				var adjacentHexes = FindHexesWithin1(hex.Row, hex.Column);
				if (!hex.PlanetType.HasValue || hex.PlanetType == PlanetType.Gaia || hex.PlanetType == PlanetType.Transdim)
				{
					continue;
				}
				var withSameBasePlanet = adjacentHexes
					.Where(h => h.PlanetType.HasValue && h.PlanetType == hex.PlanetType)
					.Select(h => h.Id)
					.ToList();
				if (withSameBasePlanet.Any())
				{
					return (false, new List<string> { hex.Id }.Concat(withSameBasePlanet).ToList());
				}
			}
			return (true, null);
		}

		private void FixInvalidHexes(IEnumerable<string> invalidHexIds)
		{
			var sectorsWithRotation = invalidHexIds
				.Select(hexId => Map.Hexes.First(h => h.Id == hexId))
				.Select(hex => new { hex.SectorId, hex.SectorRotation })
				.DistinctBy(hex => hex.SectorId)
				.ToList();
			Debug.Assert(1 < sectorsWithRotation.Count && sectorsWithRotation.Count <= 3, "Conflicting sectors must be 2 or 3!");
			bool clockwise = true;
			foreach (var sector in sectorsWithRotation.Skip(1))
			{
				RotateSector(sector.SectorId, 1, clockwise);
				clockwise = !clockwise;
			}
		}

		public void RotateSector(string sectorId, int steps, bool clockwise)
		{
			var firstHexInSector = Map.Hexes
				.Where(h => h.SectorId == sectorId)
				.MinBy(h => h.Index)
				.First();
			var sectorNumber = firstHexInSector.SectorNumber;
			var currentRotation = firstHexInSector.SectorRotation;
			var newRotationTmp = clockwise
				? currentRotation - (steps % MaxRotationSteps)
				: currentRotation + steps;
			newRotationTmp = newRotationTmp < 0 ? (MaxRotationSteps - newRotationTmp) : newRotationTmp;
			var newRotation = (newRotationTmp % MaxRotationSteps);
			var sectorData = _mapData.Sectors.First(s => s.Id == sectorId);
			var sectorPosition = _mapShapeData.SectorPositions[sectorNumber];
			var newHexes = CreateHexes(sectorData, sectorPosition, sectorNumber, newRotation);
			Map.Hexes = Map.Hexes.Where(h => h.SectorId != sectorId).Concat(newHexes).OrderBy(h => h.SectorId).ToList();
		}

		/// <summary>
		/// This method returns a list of all hexes that, together with the specified one,
		/// form a cluster of constructions of the same player. This is only needed when expanding federations
		/// so, if the player is Ivits, the cluster shall include space stations
		/// </summary>
		/// <param name="hex"></param>
		/// <param name="playerId"></param>
		/// <returns></returns>
		public List<Hex> GetBuildingCluster(Hex hex, string playerId)
		{
			if (!hex.Buildings.Any(b => b.PlayerId == playerId))
			{
				return new List<Hex>();
			}
			var cluster = new List<Hex> { hex };
			var adjacentBuildings = GetAdjacentHexes(hex)
				.WithPlayer(playerId)
				.WithFederatableBuildings()
				.ToArray();
			while (adjacentBuildings.Any())
			{
				cluster.AddRange(adjacentBuildings);
				adjacentBuildings = adjacentBuildings
					.SelectMany(h => GetAdjacentHexes(h))
					.Where(h => cluster.All(hc => hc.Id != h.Id))
					.WithPlayer(playerId)
					.WithFederatableBuildings()
					.ToArray();
			}
			return cluster;
		}

		public List<Hex> GetBuildingCluster(string hexId, string playerId)
		{
			var hex = GetHex(hexId);
			return GetBuildingCluster(hex, playerId);
		}

		public List<Hex> GetAdjacentHexes(string hexId)
		{
			var hex = GetHex(hexId);
			return FindHexesWithin1(hex.Row, hex.Column);
		}

		public List<Hex> GetAdjacentHexes(Hex hex)
		{
			return FindHexesWithin1(hex.Row, hex.Column);
		}

		public Hex GetHex(string id)
		{
			return Map.Hexes.Single(h => h.Id == id);
		}

		public Building[] GetBuildingsWithinDistance(Hex targetHex, int distance)
		{
			var surroundingHexes = FindHexesWithinDistance(targetHex, distance);
			return surroundingHexes
				.Concat(new List<Hex> { targetHex })
				.Where(h => h.PlanetType.HasValue)
				.SelectMany(h => h.Buildings)
				.ToArray();
		}

		public List<Hex> GetColonizableHexesOfType(PlanetType type)
		{
			return Map.Hexes
				.OfType(type)
				.Colonizable()
				.ToList();
		}

		public List<Hex> GetPlayersHexes(string playerId, bool includeWithGaiaformer = false, bool includeWithLantidsMine = true)
		{
			var ret = Map.Hexes.WithPlayer(playerId);
			if (!includeWithGaiaformer)
			{
				ret = ret.Except(Map.Hexes.WithPlayer(playerId).WithGaiaformer());
			}
			if (!includeWithLantidsMine)
			{
				ret = ret.Except(Map.Hexes.WithPlayer(playerId).WithLantidsMine());
			}
			return ret.ToList();
		}

		public List<(Hex hex, int requiredQics)> GetHexesColonizableBy(PlayerInGame player)
		{
			var availableQics = player.State.Resources.Qic;
			var isLantids = player.RaceId == Race.Lantids;
			var playersRange = player.State.NavigationRange;
			var allPlayerHexes = GetPlayersHexes(player.Id, true);
			var playerHexesWithNewGaiaformer = allPlayerHexes.WithGaiaformer().Gaiaformed(false).ToArray();
			var playerHexesWithGaiaformer = allPlayerHexes.WithGaiaformer().Gaiaformed(true).ToArray();
			var playerHexesWithStructure = allPlayerHexes
				.Except(playerHexesWithNewGaiaformer)
				.Except(playerHexesWithGaiaformer)
				.Except(allPlayerHexes.WithSatellites())
				.ToArray();
			var hexesInRangeWithoutBoost = playerHexesWithStructure
				.SelectMany(h => FindHexesWithinDistance(h, playersRange))
				.Colonizable(isLantids)
				.ExceptBy(playerHexesWithStructure, h => h.Id)
				.Concat(playerHexesWithGaiaformer)
				.DistinctBy(h => h.Id)
				.ToList();

			var ret = hexesInRangeWithoutBoost.Select(hex => (hex, 0)).ToList();
			var rangeBoost = 0;
			while (availableQics > 0)
			{
				rangeBoost += RangeBoostFromQic;
				var actualRange = playersRange + rangeBoost;
				--availableQics;
				var additionalHexes = playerHexesWithStructure
					.SelectMany(h => FindHexesWithinDistance(h, actualRange))
					.Colonizable(isLantids)
					.ExceptBy(playerHexesWithStructure, h => h.Id)
					.DistinctBy(h => h.Id)
					.ToList();
				ret.AddRange(
					additionalHexes
						.Where(h => !ret.Exists(o => o.hex.Id == h.Id))
						.Select(h => (h, rangeBoost / RangeBoostFromQic))
				);
			}
			return ret;
		}

		public List<(Hex hex, int requiredQics)> GetHexesReachableBy(PlayerInGame player)
		{
			var availableQics = player.State.Resources.Qic;
			var playerHexes = GetPlayersHexes(player.Id);
			var playersRange = player.State.NavigationRange;
			var hexesReachableWithoutBoost = playerHexes
				.SelectMany(h => FindHexesWithinDistance(h, playersRange))
				.Except(playerHexes.WithSatellites())
				.ExceptBy(playerHexes, h => h.Id)
				.DistinctBy(h => h.Id)
				.ToList();

			var ret = hexesReachableWithoutBoost.Select(h => (h, 0)).ToList();
			var rangeBoost = 0;
			while (availableQics > 0)
			{
				rangeBoost += RangeBoostFromQic;
				var actualRange = playersRange + rangeBoost;
				--availableQics;
				var additionalHexes = playerHexes
					.SelectMany(h => FindHexesWithinDistance(h, actualRange))
					.ExceptBy(playerHexes, h => h.Id)
					.DistinctBy(h => h.Id)
					.ToList();
				ret.AddRange(
					additionalHexes
						.Where(h => !ret.Exists(o => o.h.Id == h.Id))
						.Select(h => (h, rangeBoost / RangeBoostFromQic))
				);
			}
			return ret;
		}

		public List<Hex> FindHexesWithinDistance(Hex hex, int distance)
		{
			if (distance < 0)
			{
				throw new ArgumentOutOfRangeException("Distance cannot be negative.");
			}
			if (distance == 0)
			{
				return new List<Hex>();
			}

			return Map.Hexes
				.Where(h => h.Id != hex.Id
					&& Math.Abs(h.Column - hex.Column) <= distance
					&& Math.Abs(h.Row - hex.Row) + Math.Abs(h.Column - hex.Column) <= 2 * distance
				)
				.ToList();
		}

		public List<Hex> FindHexesWithinDistance(int row, int column, int distance)
		{
			return FindHexesWithinDistance(GetHexAt(row, column), distance);
		}

		private List<Hex> FindHexesWithin1(int row, int column)
		{
			return FindHexesWithinDistance(row, column, 1);
		}

		public Hex GetHexAt(int row, int column)
		{
			return Map.Hexes.SingleOrDefault(h => h.Row == row && h.Column == column);
		}

		private Hex[] CreateHexes(SectorData sector, ElementPosition sectorPosition, int sectorNumber, int? rotation = null)
		{
			var hexPositions = _mapData.HexPositions;
			var sectorRotation = rotation ?? ThreadSafeRandom.ThisThreadsRandom.Next(0, MaxRotationSteps);
			var rotatedIndexes = GetRotatedHexIndexes(sectorRotation);
			short hexIndex = 0;
			var ret = hexPositions.Select(hp =>
			{
				// e.g.: At hex #0, we must place the planet which usually is in the hex that, when rotated, goes to 0
				var reverseRotatedIndex = rotatedIndexes[hexIndex];
				//var reverseRotatedIndex = rotatedIndexes.IndexOf(hexIndex);
				var planetData = sector.Planets.FirstOrDefault(p => p.HexIndex == reverseRotatedIndex);
				var planetType = planetData?.PlanetType;
				return Hex.Create(
					sector.Id, sectorNumber, sectorRotation,
					hexIndex++,
					(sectorPosition.Row + hp.Row),
					(sectorPosition.Column + hp.Column),
					planetType
				);
			})
			.ToArray();
			return ret;
		}

		/** Get the indexes of hexes given a certain rotation.
			Such indexes describe which hex ends up in each location (from top left to bottom right).
			Rotation is measured counter-clockwise and in n. of 60° steps,
			so e.g. 2 means 120° counter-clockwise
		*/
		private List<int> GetRotatedHexIndexes(int rotationSteps)
		{
			var actualSequence = _rotationOffsetsHalfSequences[rotationSteps % MaxRotationSteps];
			const int halfSequenceLength = 9;   // Middle point
			const int wholeSequenceLength = 19; // A sector has 19 hexes
			var rotatedIndexes = new List<int>();
			for (var i = 0; i < wholeSequenceLength; ++i)
			{
				if (i < halfSequenceLength)
				{
					rotatedIndexes.Add(i + actualSequence[i]);
					continue;
				}
				else if (i == halfSequenceLength)
				{
					rotatedIndexes.Add(halfSequenceLength);
				}
				else
				{
					var reverseI = 2 * halfSequenceLength - i;
					rotatedIndexes.Add(i - actualSequence[reverseI]);
				}
			}
			return rotatedIndexes;
		}

		public bool IsInRangeOfOtherPlayers(Hex hex, string playerId, int range)
		{
			var hexesWithOtherPlayersBuildings = FindHexesWithinDistance(hex, range).WithPlayersOtherThan(playerId);
			return hexesWithOtherPlayersBuildings
				.Except(hexesWithOtherPlayersBuildings.WithBuildingType(BuildingType.IvitsSpaceStation))
				.Except(hexesWithOtherPlayersBuildings.WithBuildingType(BuildingType.Satellite))
				.Any();
		}
	}
}
