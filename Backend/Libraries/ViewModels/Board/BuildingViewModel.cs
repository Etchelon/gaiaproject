using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
    public class BuildingViewModel
    {
        public string Username { get; set; }
        public Race RaceId { get; set; }
        public BuildingType Type { get; set; }
        public int PowerValue { get; set; }
        public int PowerValueInFederation { get; set; }
        public string FederationId { get; set; }
        public bool ShowFederationMarker { get; set; }
    }
}