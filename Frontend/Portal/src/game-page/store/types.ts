import { HubConnectionState } from "@microsoft/signalr";
import { AvailableActionDto, GameStateDto } from "../../dto/interfaces";
import { LoadingStatus } from "../../games/store/types";
import { Nullable } from "../../utils/miscellanea";
import { ActiveView, Command, InteractiveElement } from "../workflows/types";

export interface ActiveGameSliceState {
	currentUserId: string;
	gameState: Nullable<GameStateDto>;
	activeView: ActiveView;
	statusMessage: string;
	availableCommands: Command[];
	interactiveElements: InteractiveElement[];
	availableActions: AvailableActionDto[];
	status: LoadingStatus;
	actionProgress: LoadingStatus;
	rollbackProgress: LoadingStatus;
	hubState: {
		connectionState: HubConnectionState;
		connectedToGameId: Nullable<string>;
		onlineUsers: string[];
	};

	//#region Player Notes
	playerNotes: Nullable<string>;
	saveNotesProgress: LoadingStatus;
	//#endregion

	// TODO: game settings (like board and buildings style, autocharging, maybe premoves one day...)
	playerSettings?: any;
}
