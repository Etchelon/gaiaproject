using System;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class BuildingDeployedEffect : Effect
	{
		public BuildingType BuildingType { get; set; }
		public string HexId { get; set; }
		public BuildingType? RemovedBuildingType { get; }

		public BuildingDeployedEffect(BuildingType type, string hexId, BuildingType? removedBuildingType = null)
		{
			BuildingType = type;
			HexId = hexId;
			RemovedBuildingType = removedBuildingType;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var playerBuildings = player.State.Buildings;
			var targetHex = game.BoardState.Map.Hexes.Single(h => h.Id == HexId);
			var hexBuildings = targetHex.Buildings.ToList();
			if (BuildingType == BuildingType.LostPlanet)
			{
				targetHex.PlanetType = PlanetType.LostPlanet;
				hexBuildings.Add(Building.Factory.Create(BuildingType.LostPlanet, player, targetHex.Id, PlanetType.LostPlanet));
				targetHex.Buildings = hexBuildings.ToList();
				playerBuildings.HasLostPlanet = true;
				return;
			}

			var previousBuilding = hexBuildings.FirstOrDefault(b => b.PlayerId == PlayerId);
			if (previousBuilding != null)
			{
				hexBuildings.Remove(previousBuilding);
			}
			hexBuildings.Add(Building.Factory.Create(BuildingType, player, targetHex.Id, targetHex.PlanetType));

			switch (BuildingType)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(BuildingType), $"Building type {BuildingType} not handled.");
				case BuildingType.Mine:
					playerBuildings.Mines += 1;
					var fromGaiaformer = previousBuilding?.Type == BuildingType.Gaiaformer;
					if (fromGaiaformer)
					{
						player.State = GaiaformingUtils.ReturnGaiaformerFromHex(player, HexId);
					}
					break;
				case BuildingType.TradingStation:
					playerBuildings.TradingStations += 1;
					// Guard for Firaks downgrade
					if (RemovedBuildingType == BuildingType.ResearchLab)
					{
						playerBuildings.ResearchLabs -= 1;
					}
					else
					{
						playerBuildings.Mines -= 1;
					}
					break;
				case BuildingType.ResearchLab:
					playerBuildings.ResearchLabs += 1;
					playerBuildings.TradingStations -= 1;
					break;
				case BuildingType.PlanetaryInstitute:
					playerBuildings.PlanetaryInstitute = true;
					if (player.RaceId == Race.Bescods)
					{
						playerBuildings.ResearchLabs -= 1;
					}
					else
					{
						// Guard for Ivits PI initial placement
						playerBuildings.TradingStations = playerBuildings.TradingStations == 0
							? 0
							: playerBuildings.TradingStations - 1;
					}
					break;
				case BuildingType.AcademyLeft:
				case BuildingType.AcademyRight:
					if (BuildingType == BuildingType.AcademyLeft)
					{
						playerBuildings.AcademyLeft = true;
					}
					if (BuildingType == BuildingType.AcademyRight)
					{
						playerBuildings.AcademyRight = true;
					}

					if (player.RaceId == Race.Bescods)
					{
						playerBuildings.TradingStations -= 1;
					}
					else
					{
						playerBuildings.ResearchLabs -= 1;
					}
					break;
				case BuildingType.Gaiaformer:
					player.State = GaiaformingUtils.SendGaiaformerToHex(player, HexId);
					break;
				case BuildingType.Satellite:
					playerBuildings.Satellites += 1;
					break;
				case BuildingType.IvitsSpaceStation:
					playerBuildings.IvitsSpaceStations += 1;
					break;
			}

			targetHex.Buildings = hexBuildings.ToList();
		}
	}
}
