using System.Linq;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
    [BsonNoId]
    public class Building
    {
        public string Id { get; set; }
        public BuildingType Type { get; set; }
        public string PlayerId { get; set; }
        public Race RaceId { get; set; }
        public string HexId { get; set; }
        public int PowerValue { get; set; }

        [BsonIgnoreIfDefault]
        public bool ShowFederationMarker { get; set; }

        public int PowerValueInFederation => RaceId == Race.Ivits && Type == BuildingType.IvitsSpaceStation
            ? PowerValue + 1
            : PowerValue;

        private Building() { }

        public static class Factory
        {
            public static Building Create(BuildingType type, PlayerInGame player, string hexId, PlanetType? planetType)
            {
                return new Building
                {
                    Id = $"{hexId}_{player.Id}",
                    Type = type,
                    PlayerId = player.Id,
                    RaceId = player.RaceId!.Value,
                    HexId = hexId,
                    PowerValue = GetActualPowerValue(type, planetType, player)
                };
            }

            private static int GetActualPowerValue(BuildingType type, PlanetType? planetType, PlayerInGame player)
            {
                int value;
                switch (type)
                {
                    default:
                    case BuildingType.Gaiaformer:
                    case BuildingType.Satellite:
                    case BuildingType.IvitsSpaceStation:
                        return 0;
                    case BuildingType.LostPlanet:
                        return 1;
                    case BuildingType.Mine:
                        value = 1;
                        break;
                    case BuildingType.TradingStation:
                    case BuildingType.ResearchLab:
                        value = 2;
                        break;
                    case BuildingType.PlanetaryInstitute:
                    case BuildingType.AcademyLeft:
                    case BuildingType.AcademyRight:
                        var hasStandardTile = player.State.StandardTechnologyTiles.Any(st => st.Id == StandardTechnologyTileType.PassiveBigBuildingsWorth4Power);
                        value = hasStandardTile ? 4 : 3;
                        break;
                }
                var isBescodsWithPlanetaryInstitute = player.RaceId == Race.Bescods && player.State.Buildings.PlanetaryInstitute;
                return value + (isBescodsWithPlanetaryInstitute && planetType == PlanetType.Titanium ? 1 : 0);
            }
        }
    }
}
