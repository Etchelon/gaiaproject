using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.AvailableActions;
using GaiaProject.ViewModels.Decisions;

namespace GaiaProject.ViewModels.Players
{
    public class ActivePlayerInfoViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Race? RaceId { get; set; }
        public string Reason { get; set; }
        public List<AvailableActionViewModel> AvailableActions { get; set; }
        public PendingDecisionViewModel PendingDecision { get; set; }
    }
}