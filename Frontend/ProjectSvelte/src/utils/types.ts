export enum ActiveView {
	Map,
	ResearchBoard,
	ScoringBoard,
	PlayerArea,
	NotesAndSettings,
	RaceSelectionDialog,
	AuctionDialog,
	ConversionDialog,
	SortIncomesDialog,
	TerransConversionsDialog,
}

export interface ElementSize {
	width: number;
	height: number;
}

export enum InteractiveElementState {
	Disabled,
	Enabled,
	Selected,
}

export enum InteractiveElementType {
	Hex,
	RoundBooster,
	ResearchStep,
	StandardTile,
	AdvancedTile,
	PowerAction,
	QicAction,
	FederationToken,
	OwnStandardTile,
	OwnAdvancedTile,
	OwnRoundBooster,
	OwnFederationToken,
	PlanetaryInstitute,
	RightAcademy,
	RaceAction,
}
