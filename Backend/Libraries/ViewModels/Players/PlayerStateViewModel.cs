using System.Collections.Generic;
using System.Linq;

namespace GaiaProject.ViewModels.Players
{
	public class PlayerStateViewModel
	{
		public int CurrentRoundTurnOrder { get; set; }
		public int? NextRoundTurnOrder { get; set; }
		public bool HasPassed { get; set; }
		public int Points { get; set; }
		public int? AuctionPoints { get; set; }
		public int TerraformingCost { get; set; }
		public int? TempTerraformingSteps { get; set; }
		public int NavigationRange { get; set; }
		public int? RangeBoost { get; set; }
		public ResearchAdvancementsViewModel ResearchAdvancements { get; set; }
		public List<GaiaformerViewModel> AvailableGaiaformers { get; set; }
		public ResourcesViewModel Resources { get; set; }
		public IncomeViewModel Income { get; set; }
		public DeployedBuildingsViewModel Buildings { get; set; }
		public List<TechnologyTileViewModel> TechnologyTiles { get; set; }
		public RoundBoosterViewModel RoundBooster { get; set; }
		public SpecialActionSpaceViewModel PlanetaryInstituteActionSpace { get; set; }
		public SpecialActionSpaceViewModel RightAcademyActionSpace { get; set; }
		public SpecialActionSpaceViewModel RaceActionSpace { get; set; }
		public List<FederationTokenViewModel> FederationTokens { get; set; }
		public int UsableGaiaformers => AvailableGaiaformers?.Where(g => g.Available).Count() ?? 0;
		public int UnlockedGaiaformers => AvailableGaiaformers?.Count() ?? 0;
		public int UsableFederations => FederationTokens?.Where(f => !f.UsedForTechOrAdvancedTile).Count() ?? 0;
		public int NumFederationTokens => FederationTokens?.Count() ?? 0;
		public int KnownPlanetTypes { get; set; }
		public int ColonizedSectors { get; set; }
		public int GaiaPlanets { get; set; }
		public string AdditionalInfo { get; set; }
	}
}