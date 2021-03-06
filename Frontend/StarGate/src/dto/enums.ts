export enum MapShape {
	Custom,
	Standard1P,
	Standard2P,
	Standard3P,
	Standard4P,
	IntroductoryGame2P,
	IntroductoryGame34P,
}

export enum TurnOrderSelectionMode {
	Random,
	Assigned,
	Auction,
}

export enum RaceSelectionMode {
	Random,
	TurnOrder,
}

export enum RoundBoosterType {
	GainOreGainKnowledge,
	GainPowerTokensGainOre,
	GainCreditsGainQic,
	TerraformActionGainCredits,
	BoostRangeGainPower,
	PassPointsPerMineGainOre,
	PassPointsPerTradingStationsGainOre,
	PassPointsPerResearchLabsGainKnowledge,
	PassPointsPerBigBuildingsGainPower,
	PassPointsPerGaiaPlanetsGainCredits,
}

export enum StandardTechnologyTileType {
	ActionGain4Power,
	Immediate1Ore1Qic,
	Immediate1KnowledgePerPlanetType,
	Immediate7Points,
	Income1Ore1Power,
	Income1Knowledge1Coin,
	Income4Coins,
	PassiveBigBuildingsWorth4Power,
	Passive3PointsPerGaiaPlanet,
}

export enum AdvancedTechnologyTileType {
	ActionGain1Qic5Credits,
	ActionGain3Ores,
	ActionGain3Knowledge,
	Immediate2PointsPerMine,
	Immediate4PointsPerTradingStation,
	Immediate5PointsPerFederation,
	Immediate2PointsPerSector,
	Immediate2PointsPerGaiaPlanet,
	Immediate1OrePerSector,
	Pass3PointsPerFederation,
	Pass3PointsPerResearchLab,
	Pass1PointsPerPlanetType,
	Passive2PointsPerResearchStep,
	Passive3PointsPerMine,
	Passive3PointsPerTradingStation,
}

export enum PowerActionType {
	Gain3Knowledge,
	Gain2TerraformingSteps,
	Gain2Ores,
	Gain7Credits,
	Gain2Knowledge,
	Gain1TerraformingStep,
	Gain2PowerTokens,
}

export enum QicActionType {
	GainTechnologyTile,
	RescoreFederationBonus,
	GainPointsPerPlanetTypes,
}

export enum FederationTokenType {
	Knowledge,
	Credits,
	Ores,
	PowerTokens,
	Qic,
	Points,
	Gleens = 42,
}

export enum ResearchTrackType {
	Terraformation,
	Navigation,
	ArtificialIntelligence,
	Gaiaformation,
	Economy,
	Science,
}

export enum ActionType {
	AdjustSector,
	SelectRace,
	BidForRace,
	PlaceInitialStructure,
	SelectStartingRoundBooster,
	Conversions,
	ColonizePlanet,
	IvitsPlaceSpaceStation,
	StartGaiaProject,
	UpgradeExistingStructure,
	FiraksDowngradeResearchLab,
	AmbasSwapPlanetaryInstituteAndMine,
	FormFederation,
	ResearchTechnology,
	BescodsResearchProgress,
	Pass,
	PassTurn,
	Power,
	Qic,
	ChooseTechnologyTile,
	ChargePower,
	TerransDecideIncome,
	ItarsBurnPowerForTechnologyTile,
	UseRightAcademy,
	SortIncomes,
	UseTechnologyTile,
	UseRoundBooster,
	PlaceLostPlanet,
	RescoreFederationToken,
	TaklonsLeech,
	AcceptOrDeclineLastStep,
}

export enum PendingDecisionType {
	ChargePower,
	PlaceLostPlanet,
	TerransDecideIncome,
	ItarsBurnPowerForTechnologyTile,
	PerformConversionOrPassTurn,
	SortIncomes,
	FreeTechnologyStep,
	ChooseTechnologyTile,
	SelectFederationTokenToScore,
	TaklonsLeech,
	AcceptOrDeclineLastStep,
}

export enum BuildingType {
	Mine,
	TradingStation,
	PlanetaryInstitute,
	ResearchLab,
	AcademyLeft,
	AcademyRight,
	Gaiaformer,
	Satellite,
	LostPlanet,
	IvitsSpaceStation,
}

export enum GamePhase {
	Setup,
	Rounds,
}

export enum Race {
	None,
	Terrans,
	Lantids,
	Taklons,
	Ambas,
	Gleens,
	Xenos,
	Ivits,
	HadschHallas,
	Bescods,
	Firaks,
	Geodens,
	BalTaks,
	Nevlas,
	Itars,
}

export enum PlanetType {
	Terra,
	Swamp,
	Desert,
	Oxide,
	Titanium,
	Volcanic,
	Ice,
	Gaia,
	Transdim,
	LostPlanet,
}

export enum RoundScoringTileType {
	PointsPerTerraformingStep2,
	PointsPerResearchStep2,
	PointsPerMine2,
	PointsPerTradingStation3,
	PointsPerTradingStation4,
	PointsPerGaiaPlanet3,
	PointsPerGaiaPlanet4,
	PointsPerBigBuilding5,
	PointsPerBigBuilding5Bis,
	PointsPerFederation5,
}

export enum FinalScoringTileType {
	BuildingsInAFederation,
	BuildingsOnTheMap,
	KnownPlanetTypes,
	GaiaPlanets,
	Sectors,
	Satellites,
}

export enum BrainstoneLocation {
	Removed,
	Bowl1,
	Bowl2,
	Bowl3,
	GaiaArea,
}

export enum Conversion {
	PowerToQic,
	PowerToOre,
	PowerToKnowledge,
	PowerToCredit,
	OreToCredit,
	OreToPowerToken,
	KnowledgeToCredit,
	QicToOre,
	BoostRange,
	BurnPower,
	NevlasPower3ToKnowledge,
	Nevlas3PowerTo2Ores,
	Nevlas2PowerToOreAndCredit,
	Nevlas2PowerToQic,
	Nevlas2PowerToKnowledge,
	NevlasPowerTo2Credits,
	BalTaksGaiaformerToQic,
	HadschHallas4CreditsToQic,
	HadschHallas3CreditsToOre,
	HadschHallas4CreditsToKnowledge,
	TaklonsBrainstoneToCredits,
}

export enum SortableIncomeType {
	Power,
	PowerToken,
}
