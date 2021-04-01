using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Board;
using MoreLinq.Extensions;

namespace GaiaProject.Engine.Logic.Board.Map
{
	public static class HexesExtensions
	{
		/// <summary>
		/// Filters hexes returning all that have no buildings on it, of any type
		/// </summary>
		/// <param name="hexes"></param>
		/// <returns></returns>
		public static IEnumerable<Hex> Empty(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => !h.Buildings.Any());
		}

		public static IEnumerable<Hex> EmptyOrWithSatellites(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => !h.Buildings.Any() || h.Buildings.All(b => b.Type == BuildingType.Satellite));
		}

		public static IEnumerable<Hex> WithSatellites(this IEnumerable<Hex> hexes, bool includeIvitsSpaceStations = false)
		{
			var allowedTypes = new List<BuildingType> { BuildingType.Satellite };
			if (includeIvitsSpaceStations)
			{
				allowedTypes.Add(BuildingType.IvitsSpaceStation);
			}
			return hexes.Where(h => h.Buildings.Any(b => allowedTypes.Contains(b.Type)));
		}

		public static IEnumerable<Hex> WithIvitsSpaceStation(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => h.Buildings.FirstOrDefault()?.Type == BuildingType.IvitsSpaceStation);
		}

		public static IEnumerable<Hex> Colonizable(this IEnumerable<Hex> hexes, bool alsoWithLantidsMine = false)
		{
			static bool PredNormal(Hex h) => !h.Buildings.Any();
			static bool PredLantids(Hex h) => !h.Buildings.Any() || h.Buildings.Count == 1 && h.Buildings.Single().RaceId != Race.Lantids;
			return hexes.Except(hexes.Space()).Where(alsoWithLantidsMine ? (Func<Hex, bool>)PredLantids : PredNormal);
		}

		public static IEnumerable<Hex> WithPlayer(this IEnumerable<Hex> hexes, string playerId)
		{
			return hexes.Where(h => h.Buildings.Any(b => b.PlayerId == playerId));
		}

		public static IEnumerable<Hex> WithPlayersOtherThan(this IEnumerable<Hex> hexes, string playerId)
		{
			return hexes.Where(h => h.Buildings.Any(b => b.PlayerId != playerId));
		}

		public static IEnumerable<Hex> WithLantidsMine(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => h.Buildings.Count == 2);
		}

		public static IEnumerable<Hex> WithGaiaformer(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => h.Buildings.FirstOrDefault()?.Type == BuildingType.Gaiaformer);
		}

		public static IEnumerable<Hex> OfType(this IEnumerable<Hex> hexes, PlanetType type)
		{
			return hexes.Where(h => h.ActualPlanetType == type);
		}

		public static IEnumerable<Hex> WithConcretePlanet(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => h.ActualPlanetType.HasValue && h.ActualPlanetType != PlanetType.Transdim);
		}

		public static IEnumerable<Hex> Space(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => !h.PlanetType.HasValue);
		}

		public static IEnumerable<Hex> Gaia(this IEnumerable<Hex> hexes)
		{
			return hexes.OfType(PlanetType.Gaia);
		}

		public static IEnumerable<Hex> Transdim(this IEnumerable<Hex> hexes)
		{
			return hexes.OfType(PlanetType.Transdim);
		}

		public static IEnumerable<Hex> Gaiaformed(this IEnumerable<Hex> hexes, bool gaiaformed = true)
		{
			return hexes.Where(h => (h.WasGaiaformed ?? false) == gaiaformed);
		}

		public static IEnumerable<Hex> WithBuildings(this IEnumerable<Hex> hexes)
		{
			return hexes.Where(h => h.Buildings.Any());
		}

		public static IEnumerable<Hex> WithBuildingType(this IEnumerable<Hex> hexes, BuildingType type)
		{
			return hexes.Where(h => h.Buildings.Any(b => b.Type == type));
		}

		public static IEnumerable<Hex> WithFederatableBuildings(this IEnumerable<Hex> hexes)
		{
			return hexes.WithBuildings().Except(hexes.WithBuildingType(BuildingType.Gaiaformer));
		}

		public static IEnumerable<Hex> InSector(this IEnumerable<Hex> hexes, int sectorNumber)
		{
			return hexes.Where(h => h.SectorNumber == sectorNumber);
		}

		public static IEnumerable<List<Hex>> NotEmpty(this IEnumerable<List<Hex>> clusters)
		{
			return clusters.Where(c => c.Any());
		}

		public static IEnumerable<List<Hex>> Distinct(this IEnumerable<List<Hex>> clusters)
		{
			var ret = new List<List<Hex>>();
			if (!clusters.Any())
			{
				return ret;
			}

			var clusterIds = clusters.Select(hexes => string.Join("§", hexes
				.OrderBy(h => h.Id)
				.Select(h => h.Id)
				.ToArray()
			))
			.ToList();
			return clusterIds.Zip(clusters, (id, cluster) => new { Id = id, Cluster = cluster })
				.DistinctBy(o => o.Id)
				.Select(o => o.Cluster)
				.ToList();
			//var processedClusters = new List<List<Hex>>();
			//var duplicateClusters = new List<List<Hex>>();
			//ret.AddRange(clusters
			//	.Where(cluster =>
			//	{
			//		if (duplicateClusters.Contains(cluster))
			//		{
			//			return false;
			//		}
			//		var otherClusters = clusters.Except(processedClusters).ToList();
			//		otherClusters.Remove(cluster);
			//		foreach (var otherCluster in otherClusters)
			//		{
			//			var otherClusterSize = otherCluster.Count;
			//			var nIntersected = otherCluster.Select(h => h.Id).Intersect(cluster.Select(h => h.Id)).Count();
			//			if (otherClusterSize == nIntersected)
			//			{
			//				// Found a duplicate; add this one and exclude the other
			//				duplicateClusters.Add(otherCluster);
			//				processedClusters.Add(cluster);
			//				return true;
			//			}
			//			if (0 < nIntersected && nIntersected < otherClusterSize)
			//			{
			//				throw new Exception("Clusters cannot overlap");
			//			}
			//		}
			//		processedClusters.Add(cluster);
			//		return true;
			//	})
			//);
			//return ret;
		}
	}
}
