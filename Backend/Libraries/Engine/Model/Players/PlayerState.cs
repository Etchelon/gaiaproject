using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class PlayerState
	{
		public int CurrentRoundTurnOrder { get; set; }
		public int? NextRoundTurnOrder { get; set; }
		public int Points { get; set; }

		[BsonIgnoreIfNull]
		public int? AuctionPoints { get; set; }
		public int TerraformingCost { get; set; } = 3;
		public int? TempTerraformationSteps { get; set; }
		public int Range { get; set; } = 1;
		public int? RangeBoost { get; set; }
		public int NavigationRange => Range + (RangeBoost ?? 0);
		public int GaiaPlanets { get; set; }
		public List<string> ColonizedSectors { get; set; } = new List<string>();
		public List<PlanetType> KnownPlanetTypes { get; set; } = new List<PlanetType>();
		public List<ResearchAdvancement> ResearchAdvancements { get; set; } = new List<ResearchAdvancement>();
		public List<Gaiaformer> Gaiaformers { get; set; } = new List<Gaiaformer>();
		public PlayerResources Resources { get; set; }
		public List<Income> Incomes { get; set; } = new List<Income>();
		public DeployedBuildings Buildings { get; set; }
		public List<StandardTechnologyTile> StandardTechnologyTiles { get; set; } = new List<StandardTechnologyTile>();
		public List<AdvancedTechnologyTile> AdvancedTechnologyTiles { get; set; } = new List<AdvancedTechnologyTile>();
		public List<Federation> Federations { get; set; } = new List<Federation>();
		public List<FederationToken> FederationTokens { get; set; } = new List<FederationToken>();

		[BsonIgnoreIfNull]
		public RoundBooster RoundBooster { get; set; }

		public PlayerState Clone()
		{
			return new PlayerState
			{
				CurrentRoundTurnOrder = CurrentRoundTurnOrder,
				NextRoundTurnOrder = NextRoundTurnOrder,
				Points = Points,
				AuctionPoints = AuctionPoints,
				TerraformingCost = TerraformingCost,
				TempTerraformationSteps = TempTerraformationSteps,
				Range = Range,
				RangeBoost = RangeBoost,
				KnownPlanetTypes = KnownPlanetTypes.ToList(),
				GaiaPlanets = GaiaPlanets,
				ColonizedSectors = ColonizedSectors.ToList(),
				ResearchAdvancements = ResearchAdvancements.Select(ra => ra.Clone()).ToList(),
				Gaiaformers = Gaiaformers.Select(gf => gf.Clone()).ToList(),
				Resources = Resources?.Clone(),
				Incomes = Incomes.Select(inc => inc.Clone()).ToList(),
				Buildings = Buildings?.Clone(),
				StandardTechnologyTiles = StandardTechnologyTiles.Select(stt => stt.Clone()).ToList(),
				AdvancedTechnologyTiles = AdvancedTechnologyTiles.Select(att => att.Clone()).ToList(),
				RoundBooster = RoundBooster?.Clone(),
				Federations = Federations?.Select(fed => fed.Clone()).ToList(),
				FederationTokens = FederationTokens.Select(fed => fed.Clone()).ToList()
			};
		}
	}
}