export type GameKind = "active" | "finished";

export type LoadingStatus = "idle" | "loading" | "success" | "failure";

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

const DIALOG_VIEWS = [
	ActiveView.RaceSelectionDialog,
	ActiveView.AuctionDialog,
	ActiveView.ConversionDialog,
	ActiveView.SortIncomesDialog,
	ActiveView.TerransConversionsDialog,
];
export const isDialogView = (view: ActiveView) => DIALOG_VIEWS.includes(view);

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
