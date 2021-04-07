import _ from "lodash";
import {
	AdvancedTechnologyTileType,
	BuildingType,
	FederationTokenType,
	PowerActionType,
	QicActionType,
	Race,
	ResearchTrackType,
	RoundBoosterType,
	StandardTechnologyTileType,
} from "../dto/enums";
import { ActiveView } from "../game-page/workflows/types";

type LocalizableEnum =
	| "AdvancedTechnologyTileType"
	| "BuildingType"
	| "FederationTokenType"
	| "StandardTechnologyTileType"
	| "Race"
	| "ResearchTrackType"
	| "PowerActionType"
	| "QicActionType"
	| "RoundBoosterType"
	| "ActiveView";

const advancedTiles = new Map<AdvancedTechnologyTileType, string>([
	[AdvancedTechnologyTileType.ActionGain1Qic5Credits, "ACT: 1 Qic 5 Credits"],
	[AdvancedTechnologyTileType.ActionGain3Ores, "ACT: 3 Ores"],
	[AdvancedTechnologyTileType.ActionGain3Knowledge, "ACT: 3 Knw"],
	[AdvancedTechnologyTileType.Immediate2PointsPerMine, "2VP x mine"],
	[AdvancedTechnologyTileType.Immediate4PointsPerTradingStation, "4VP x Trading station"],
	[AdvancedTechnologyTileType.Immediate5PointsPerFederation, "5VP x Federation"],
	[AdvancedTechnologyTileType.Immediate2PointsPerSector, "2VP x Sector"],
	[AdvancedTechnologyTileType.Immediate2PointsPerGaiaPlanet, "2VP x Gaia"],
	[AdvancedTechnologyTileType.Immediate1OrePerSector, "1 Ore x Sector"],
	[AdvancedTechnologyTileType.Pass3PointsPerFederation, "Pass: 3VP x Federation"],
	[AdvancedTechnologyTileType.Pass3PointsPerResearchLab, "Pass: 3VP x Research Lab"],
	[AdvancedTechnologyTileType.Pass1PointsPerPlanetType, "Pass: 1VP x Planet type"],
	[AdvancedTechnologyTileType.Passive2PointsPerResearchStep, "Research -> 2VP"],
	[AdvancedTechnologyTileType.Passive3PointsPerMine, "Mine -> 3VP"],
	[AdvancedTechnologyTileType.Passive3PointsPerTradingStation, "Trading Station -> 3VP"],
]);

const buildings = new Map<BuildingType, string>([
	[BuildingType.Mine, "Mine"],
	[BuildingType.TradingStation, "Trading Station"],
	[BuildingType.PlanetaryInstitute, "Planetary Institute"],
	[BuildingType.ResearchLab, "Research Lab"],
	[BuildingType.AcademyLeft, "Academy (Knw)"],
	[BuildingType.AcademyRight, "Academy (Qic)"],
	[BuildingType.Gaiaformer, "Gaiaformer"],
	[BuildingType.Satellite, "Satellite"],
	[BuildingType.LostPlanet, "Lost Planet"],
	[BuildingType.IvitsSpaceStation, "Ivits Station"],
]);

const federationTokens = new Map<FederationTokenType, string>([
	[FederationTokenType.Knowledge, "6VP, 2 Knowledge"],
	[FederationTokenType.Credits, "7VP, 6 Credits"],
	[FederationTokenType.Ores, "7VP, 2 Ores"],
	[FederationTokenType.PowerTokens, "8VP, 2 Power Tokens"],
	[FederationTokenType.Qic, "8VP, 1 QIC"],
	[FederationTokenType.Points, "12VP"],
	[FederationTokenType.Gleens, "1 Ore, 1 Knowledge, 2 Credits"],
]);

const standardTiles = new Map<StandardTechnologyTileType, string>([
	[StandardTechnologyTileType.ActionGain4Power, "ACT: +4 Power"],
	[StandardTechnologyTileType.Immediate1KnowledgePerPlanetType, "1 Knw x planet type"],
	[StandardTechnologyTileType.Immediate1Ore1Qic, "1 Ore 1 Qic"],
	[StandardTechnologyTileType.Immediate7Points, "7VP"],
	[StandardTechnologyTileType.Income1Knowledge1Coin, "+1 Credit +1 Knw"],
	[StandardTechnologyTileType.Income1Ore1Power, "+1 Ore +1 Power"],
	[StandardTechnologyTileType.Income4Coins, "+4 Credits"],
	[StandardTechnologyTileType.PassiveBigBuildingsWorth4Power, "PI-AC: 4 Power"],
	[StandardTechnologyTileType.Passive3PointsPerGaiaPlanet, "Gaia -> 3VP"],
]);

const races = new Map<Race, string>([
	[Race.None, "none"],
	[Race.Ambas, "Ambas"],
	[Race.BalTaks, "BalTaks"],
	[Race.Bescods, "Bescods"],
	[Race.Firaks, "Firaks"],
	[Race.Geodens, "Geodens"],
	[Race.Gleens, "Gleens"],
	[Race.HadschHallas, "HadschHallas"],
	[Race.Itars, "Itars"],
	[Race.Ivits, "Ivits"],
	[Race.Lantids, "Lantids"],
	[Race.Nevlas, "Nevlas"],
	[Race.Taklons, "Taklons"],
	[Race.Terrans, "Terrans"],
	[Race.Xenos, "Xenos"],
]);

const researchTracks = new Map<ResearchTrackType, string>([
	[ResearchTrackType.Terraformation, "Terraformation"],
	[ResearchTrackType.Navigation, "Navigation"],
	[ResearchTrackType.ArtificialIntelligence, "Artificial Intelligence"],
	[ResearchTrackType.Gaiaformation, "Gaiaformation"],
	[ResearchTrackType.Economy, "Economy"],
	[ResearchTrackType.Science, "Science"],
]);

const powerActions = new Map<PowerActionType, string>([
	[PowerActionType.Gain3Knowledge, "3 Knowledge"],
	[PowerActionType.Gain2TerraformingSteps, "ACT: terraform (+2 steps)"],
	[PowerActionType.Gain2Ores, "2 Ores"],
	[PowerActionType.Gain7Credits, "7 Credits"],
	[PowerActionType.Gain2Knowledge, "2 Knowledge"],
	[PowerActionType.Gain1TerraformingStep, "ACT: terraform (+1 step)"],
	[PowerActionType.Gain2PowerTokens, "2 Tokens"],
]);

const qicActions = new Map<QicActionType, string>([
	[QicActionType.GainTechnologyTile, "4 QIC -> +1 Technology Tile"],
	[QicActionType.RescoreFederationBonus, "3 QIC -> Rescore Federation Token"],
	[QicActionType.GainPointsPerPlanetTypes, "2 QIC -> Points per Planet Types"],
]);

const roundBoosters = new Map<RoundBoosterType, string>([
	[RoundBoosterType.GainOreGainKnowledge, "+1 Ore +1 Knw"],
	[RoundBoosterType.GainPowerTokensGainOre, "+2 Tokens +1 Ore"],
	[RoundBoosterType.GainCreditsGainQic, "+2 Credits +1 Qic"],
	[RoundBoosterType.TerraformActionGainCredits, "ACT: terraform; +2 Credits"],
	[RoundBoosterType.BoostRangeGainPower, "ACT: +3 Nav; +2 Power"],
	[RoundBoosterType.PassPointsPerMineGainOre, "Pass: 1VP x M; +1 Ore"],
	[RoundBoosterType.PassPointsPerTradingStationsGainOre, "Pass: 2VP x TS; +1 Ore"],
	[RoundBoosterType.PassPointsPerResearchLabsGainKnowledge, "Pass: 3VP x RL; +1 Knw"],
	[RoundBoosterType.PassPointsPerBigBuildingsGainPower, "Pass: 4VP x PI-AC; +4 Power"],
	[RoundBoosterType.PassPointsPerGaiaPlanetsGainCredits, "Pass: 1VP x Gaia; +4 Credits"],
]);

const activeViews = new Map<ActiveView, string>([
	[ActiveView.ConversionDialog, "Conversions"],
	[ActiveView.SortIncomesDialog, "Sort Power Incomes"],
	[ActiveView.RaceSelectionDialog, ""],
	[ActiveView.AuctionDialog, ""],
	[ActiveView.TerransConversionsDialog, "Convert power to resources"],
	[ActiveView.Map, "Map"],
	[ActiveView.ResearchBoard, "Research"],
	[ActiveView.ScoringBoard, "Scoring"],
]);

const dictionaries = new Map<LocalizableEnum, Map<number, string>>([
	["AdvancedTechnologyTileType", advancedTiles],
	["BuildingType", buildings],
	["FederationTokenType", federationTokens],
	["StandardTechnologyTileType", standardTiles],
	["Race", races],
	["ResearchTrackType", researchTracks],
	["PowerActionType", powerActions],
	["QicActionType", qicActions],
	["RoundBoosterType", roundBoosters],
	["ActiveView", activeViews],
]);

export function localizeEnum(value: number, enumName: LocalizableEnum): string {
	const dictionary = dictionaries.get(enumName);
	return dictionary?.get(value) ?? "NOT LOCALIZED";
}

export const localizeRoundBooster = _.partialRight(localizeEnum, "RoundBoosterType");
