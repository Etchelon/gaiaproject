using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.AvailableActions
{
	public class InteractionStateViewModel
	{
		public class ColonizableHexViewModel
		{
			public string Id { get; set; }
			public int? RequiredQics { get; set; }
		}

		public class UsablePowerActionViewModel
		{
			public PowerActionType Type { get; set; }
			public int? PowerToBurn { get; set; }
		}

		public class ResearcheableTechnologyViewModel
		{
			public ResearchTrackType Track { get; set; }
			public int NextStep { get; set; }
		}

		public List<Race> AvailableRaces { get; set; } = new List<Race>();
		public List<ColonizableHexViewModel> ClickableHexes { get; set; } = new List<ColonizableHexViewModel>();
		public List<RoundBoosterType> ClickableRoundBoosters { get; set; } = new List<RoundBoosterType>();
		public List<ResearcheableTechnologyViewModel> ClickableResearchTracks { get; set; } = new List<ResearcheableTechnologyViewModel>();
		public List<StandardTechnologyTileType> ClickableStandardTiles { get; set; } = new List<StandardTechnologyTileType>();
		public List<AdvancedTechnologyTileType> ClickableAdvancedTiles { get; set; } = new List<AdvancedTechnologyTileType>();
		public List<UsablePowerActionViewModel> ClickablePowerActions { get; set; } = new List<UsablePowerActionViewModel>();
		public List<QicActionType> ClickableQicActions { get; set; } = new List<QicActionType>();
		public List<FederationTokenType> ClickableFederations { get; set; } = new List<FederationTokenType>();
		public List<FederationTokenType> ClickableOwnFederations { get; set; } = new List<FederationTokenType>();
		public List<StandardTechnologyTileType> ClickableOwnStandardTiles { get; set; } = new List<StandardTechnologyTileType>();
		public List<AdvancedTechnologyTileType> ClickableOwnAdvancedTiles { get; set; } = new List<AdvancedTechnologyTileType>();
		public bool CanUseOwnRoundBooster { get; set; }
		public bool CanUsePlanetaryInstitute { get; set; }
		public bool CanUseRightAcademy { get; set; }
		public bool CanUseRaceAction { get; set; }
		public bool CanPerformConversions { get; set; }
	}
}
