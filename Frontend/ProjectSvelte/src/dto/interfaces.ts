import type {
	ActionType,
	AdvancedTechnologyTileType,
	BrainstoneLocation,
	BuildingType,
	FederationTokenType,
	FinalScoringTileType,
	GamePhase,
	MapShape,
	NotificationType,
	PendingDecisionType,
	PlanetType,
	PowerActionType,
	QicActionType,
	Race,
	RaceSelectionMode,
	ResearchTrackType,
	RoundBoosterType,
	RoundScoringTileType,
	SortableIncomeType,
	StandardTechnologyTileType,
	TurnOrderSelectionMode,
} from "./enums";

export interface UserInfoDto {
	id: string;
	username: string;
	avatar: string;
	firstName: string;
	lastName: string;
	memberSince: string;
}

export interface GameBaseDto {
	id: string;
	name: string;
	createdBy: UserInfoDto;
	created: string;
	ended: string | null;
	currentPhase: GamePhase;
}

export interface PlayerInfoDto {
	id: string;
	username: string;
	avatarUrl: string;
	raceId: Race | null;
	raceName: string | null;
	color: string | null;
	points: number;
	isActive: boolean;
	placement: number | null;
}

export interface GameInfoDto extends GameBaseDto {
	players: PlayerInfoDto[];
}

export interface Page<T> {
	items: T[];
	hasMore: boolean;
}

export interface InteractionStateDto {
	availableRaces?: Race[];
	clickableHexes?: ColonizableHexDto[];
	clickableRoundBoosters?: RoundBoosterType[];
	clickableResearchTracks?: ResearcheableTechnologyDto[];
	clickableStandardTiles?: StandardTechnologyTileType[];
	clickableAdvancedTiles?: AdvancedTechnologyTileType[];
	clickablePowerActions?: UsablePowerActionDto[];
	clickableQicActions?: QicActionType[];
	clickableFederations?: FederationTokenType[];
	clickableOwnFederations?: FederationTokenType[];
	clickableOwnStandardTiles?: StandardTechnologyTileType[];
	clickableOwnAdvancedTiles?: AdvancedTechnologyTileType[];
	canUseOwnRoundBooster?: boolean;
	canUsePlanetaryInstitute?: boolean;
	canUseRightAcademy?: boolean;
	canUseRaceAction?: boolean;
	canPerformConversions?: boolean;
}

export interface ColonizableHexDto {
	id: string;
	requiredQics: number | null;
}

export interface UsablePowerActionDto {
	type: PowerActionType;
	powerToBurn: number | null;
}

export interface ResearcheableTechnologyDto {
	track: ResearchTrackType;
	nextStep: number;
}

export interface AvailableActionDto {
	type: ActionType;
	description: string;
	interactionState: InteractionStateDto;
	additionalData: string;
}

export interface PendingDecisionDto {
	type: PendingDecisionType;
	description: string;
	interactionState: InteractionStateDto | null;
}

export interface ActivePlayerInfoDto {
	id: string;
	username: string;
	raceId: Race | null;
	reason: string;
	availableActions: AvailableActionDto[];
	pendingDecision: PendingDecisionDto;
}

//#region Board State

//#region Scoring Board

export interface RoundScoringTileDto {
	tileId: RoundScoringTileType;
	roundNumber: number;
	inactive: boolean;
}
export interface FinalScoringStateDto {
	tileId: FinalScoringTileType;
	players: PlayerFinalScoringStatusDto[];
}

export interface PlayerFinalScoringStatusDto {
	player: PlayerInfoDto;
	count: number;
	points: number;
}

export interface ScoringTrackDto {
	scoringTiles: RoundScoringTileDto[];
	finalScoring1: FinalScoringStateDto;
	finalScoring2: FinalScoringStateDto;
}

//#endregion

//#region Research Board

export interface PlayerAdvancementDto {
	raceId: Race;
	steps: number;
}

export interface StandardTechnologyTileStackDto {
	type: StandardTechnologyTileType;
	total: number;
	remaining: number;
}

export interface ResearchTrackDto {
	id: ResearchTrackType;
	players: PlayerAdvancementDto[];
	standardTiles: StandardTechnologyTileStackDto;
	advancedTileType: AdvancedTechnologyTileType | null;
	federation: FederationTokenType | null;
	lostPlanet: boolean;
}

type ActionSpaceTypes = PowerActionType | QicActionType | Race;
export interface ActionSpaceDto<T extends ActionSpaceTypes = ActionSpaceTypes> {
	kind: "power" | "qic" | "planetary-institute" | "right-academy" | "race";
	type: T;
	isAvailable: boolean;
}

export interface PowerActionSpaceDto extends ActionSpaceDto<PowerActionType> {}

export interface QicActionSpaceDto extends ActionSpaceDto<QicActionType> {}

export interface ResearchBoardDto {
	tracks: ResearchTrackDto[];
	freeStandardTiles: StandardTechnologyTileStackDto[];
	powerActions: PowerActionSpaceDto[];
	qicActions: QicActionSpaceDto[];
}

//#endregion

export interface RoundBoosterTileDto {
	id: RoundBoosterType;
	isTaken: boolean;
	player: PlayerInfoDto;
	used: boolean;
}

export interface FederationTokenStackDto {
	type: FederationTokenType;
	initialQuantity: number;
	remaining: number;
}

export interface BoardStateDto {
	map: MapDto;
	scoringBoard: ScoringTrackDto;
	researchBoard: ResearchBoardDto;
	availableRoundBoosters: RoundBoosterTileDto[];
	availableFederations: FederationTokenStackDto[];
}

//#endregion

export interface ResearchAdvancementsDto {
	terraformation: number;
	navigation: number;
	artificialIntelligence: number;
	gaiaformation: number;
	economy: number;
	science: number;
}

export interface GaiaformerDto {
	id: number;
	available: boolean;
	spentInGaiaArea: boolean;
	onHexId: string;
}

export interface PowerPoolsDto {
	bowl1: number;
	bowl2: number;
	bowl3: number;
	gaiaArea: number;
	brainstone: BrainstoneLocation | null;
	brainstoneSummary: string;
}

export interface ResourcesDto {
	credits: number;
	ores: number;
	knowledge: number;
	qic: number;
	power: PowerPoolsDto;
}

export interface IncomeDto {
	credits: number;
	ores: number;
	knowledge: number;
	qic: number;
	power: number;
	powerTokens: number;
}

export interface SortableIncomeDto {
	id: number;
	type: SortableIncomeType;
	amount: number;
	description: string;
}

export interface DeployedBuildingsDto {
	mines: number;
	tradingStations: number;
	researchLabs: number;
	planetaryInstitute: boolean;
	academyLeft: boolean;
	academyRight: boolean;
}

export interface TechnologyTileDto {
	id: StandardTechnologyTileType;
	coveredByAdvancedTile: AdvancedTechnologyTileType | null;
	used: boolean;
}

export interface RoundBoosterDto {
	id: RoundBoosterType;
	used: boolean;
	inactive: boolean;
}

export interface SpecialActionSpaceDto {
	kind: "planetary-institute" | "right-academy" | "race";
	type: Race;
	isAvailable: boolean;
}

export interface FederationTokenDto {
	id: FederationTokenType;
	usedForTechOrAdvancedTile: boolean;
}

export interface PlayerStateDto {
	currentRoundTurnOrder: number;
	nextRoundTurnOrder: number | null;
	hasPassed: boolean;
	points: number;
	auctionPoints: number | null;
	terraformingCost: number;
	tempTerraformingSteps: number | null;
	navigationRange: number;
	rangeBoost: number | null;
	researchAdvancements: ResearchAdvancementsDto;
	availableGaiaformers: GaiaformerDto[];
	resources: ResourcesDto;
	income: IncomeDto;
	buildings: DeployedBuildingsDto;
	technologyTiles: TechnologyTileDto[];
	roundBooster: RoundBoosterDto;
	planetaryInstituteActionSpace: SpecialActionSpaceDto;
	rightAcademyActionSpace: SpecialActionSpaceDto;
	raceActionSpace: SpecialActionSpaceDto;
	federationTokens: FederationTokenDto[];
	usableGaiaformers: number;
	unlockedGaiaformers: number;
	usableFederations: number;
	numFederationTokens: number;
	knownPlanetTypes: number;
	colonizedSectors: number;
	gaiaPlanets: number;
	additionalInfo: string;
}

export interface PlayerInGameDto {
	id: string;
	username: string;
	raceId: Race | null;
	isActive: boolean;
	placement: number | null;
	state: PlayerStateDto;
}

export interface AuctionDto {
	race: Race;
	order: number;
	playerUsername: string | null;
	points: number | null;
}

export interface AuctionStateDto {
	auctionedRaces: AuctionDto[];
}

export interface GameStateDto extends GameBaseDto {
	players: PlayerInGameDto[];
	boardState: BoardStateDto;
	activePlayer: ActivePlayerInfoDto;
	gameLogs: GameLogDto[];
	auctionState: AuctionStateDto | null;
	currentRound: number;
}

export interface GameSubLogDto {
	timestamp: string;
	message: string;
	player: string;
	race: Race | null;
}

export interface GameLogDto {
	timestamp: string;
	message: string;
	important: boolean;
	isSystem: boolean;
	actionId: number | null;
	turn: number | null;
	player: string;
	race: Race | null;
	subLogs: GameSubLogDto[];
}

export interface CreateGameCommand {
	playerIds: string[];
	options: GameOptions;
}

export interface GameOptions {
	gameName: string;
	mapShape: MapShape;
	turnOrderSelectionMode: TurnOrderSelectionMode;
	factionSelectionMode: RaceSelectionMode;
	auction: boolean;
	rotateSectorsInSetup?: boolean;
	preventSandwiching?: boolean;
	minPlayers?: number;
	maxPlayers?: number;
	startingVPs?: number;
	minutesPerMove?: number;
}

//#region Map DTOs

export interface BuildingDto {
	username: string;
	raceId: Race;
	type: BuildingType;
	powerValue: number;
	powerValueInFederation: number;
	federationId: string;
	showFederationMarker: boolean;
}

export interface HexDto {
	id: string;
	index: number;
	row: number;
	column: number;
	planetType: PlanetType | null;
	wasGaiaformed: boolean;
	building: BuildingDto;
	lantidsParasiteBuilding: BuildingDto;
	ivitsSpaceStation: BuildingDto;
	satellites: BuildingDto[];
}

export interface SectorDto {
	id: string;
	number: number;
	rotation: number;
	row: number;
	column: number;
	hexes: HexDto[];
}

export interface MapDto {
	playerCount: number;
	rows: number;
	columns: number;
	shape: MapShape;
	sectors: SectorDto[];
}

//#endregion

export interface ActionDto {
	Type: ActionType;
}

export interface IvitsExpandFederationInfoDto {
	RequiredPower: number;
	CanTakeMoreTokens: boolean;
}

export interface NotificationDto {
	id: string;
	type: NotificationType;
	timestamp: string;
	text: string;
	isRead: boolean;
}

export interface GameNotificationDto extends NotificationDto {
	gameId: string;
}
