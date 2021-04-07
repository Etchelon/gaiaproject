import { Identifier } from "../../utils/miscellanea";
import { InteractiveElementState, InteractiveElementType } from "./enums";

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

export const CommonWorkflowStates = {
	ABORT: -100,
	RESET: -42,
	CANCEL: -1,
	PERFORM_ACTION: 42,
	PERFORM_CONVERSION: 43,
};

export interface Command {
	nextState: number;
	text?: string;
	isPrimary?: boolean;
	data?: any;
}

export const CommonCommands = {
	Confirm: {
		text: "Confirm",
		nextState: CommonWorkflowStates.PERFORM_ACTION,
		isPrimary: true,
	},
	Cancel: {
		text: "Cancel",
		nextState: CommonWorkflowStates.CANCEL,
		isPrimary: false,
	},
	Abort: {
		text: "Abort",
		nextState: CommonWorkflowStates.ABORT,
		isPrimary: false,
	},
	Reset: {
		text: "Reset",
		nextState: CommonWorkflowStates.RESET,
		isPrimary: false,
	},
};

export interface InteractiveElement {
	id?: Identifier;
	type: InteractiveElementType;
	state: InteractiveElementState;
	notes?: string;
}

export interface WorkflowState {
	id: number;
	message: string;
	commands?: Command[];
	interactiveElements?: InteractiveElement[];
	view?: ActiveView;
	data?: any;
}
