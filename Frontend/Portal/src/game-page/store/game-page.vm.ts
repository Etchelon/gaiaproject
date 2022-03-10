import { HubConnectionState } from "@microsoft/signalr";
import { assign, chain, isNil, without } from "lodash";
import { makeAutoObservable, runInAction } from "mobx";
import { ActionDto, AvailableActionDto, GameStateDto, MapDto } from "../../dto/interfaces";
import { LoadingStatus } from "../../games/store/types";
import { HttpClient } from "../../utils/http-client";
import { Nullable } from "../../utils/miscellanea";
import { ActiveView, Command, InteractiveElement, WorkflowState } from "../workflows/types";

const initialState = {
	currentUserId: "",
	gameState: null,
	statusMessage: "",
	activeView: ActiveView.Map,
	availableCommands: [],
	interactiveElements: [],
	availableActions: [],
	status: "idle",
	actionProgress: "idle",
	rollbackProgress: "idle",
	hubState: {
		connectionState: HubConnectionState.Disconnected,
		connectedToGameId: null,
		onlineUsers: [],
	},
	playerNotes: null,
	saveNotesProgress: "idle",
};

interface IHubState {
	connectionState: HubConnectionState;
	connectedToGameId: Nullable<string>;
	onlineUsers: string[];
}

interface ExecuteActionResult {
	handled: boolean;
	errorMessage?: string;
}

export class GamePageViewModel {
	currentUserId: Nullable<string> = null;
	gameState: Nullable<GameStateDto> = null;
	activeView = ActiveView.Map;
	statusMessage = "";
	availableCommands: Command[] = [];
	interactiveElements: InteractiveElement[] = [];
	availableActions: AvailableActionDto[] = [];
	status: LoadingStatus = "idle";
	actionProgress: LoadingStatus = "idle";
	get isExecutingAction() {
		return this.actionProgress === "loading";
	}
	rollbackProgress: LoadingStatus = "idle";
	hubState: IHubState = {
		connectionState: HubConnectionState.Disconnected,
		connectedToGameId: null,
		onlineUsers: [],
	};
	playerNotes: Nullable<string> = null;
	saveNotesProgress: LoadingStatus = "idle";
	playerSettings?: any;

	constructor(private readonly httpClient: HttpClient) {
		makeAutoObservable(this);
	}

	setCurrentUser(id: string) {
		this.currentUserId = id;
	}

	unloadActiveGame() {
		const currentUserId = this.currentUserId;
		assign(this, initialState);
		this.currentUserId = currentUserId;
	}

	clearStatus() {
		this.activeView = ActiveView.Map;
		this.statusMessage = "";
		this.availableCommands = [];
	}

	gameUpdated(game: GameStateDto) {
		this.actionProgress = "idle";
		this.rollbackProgress = "idle";
		this.gameState = game;
		const gameIsOver = !isNil(game.ended);
		if (!gameIsOver) {
			return;
		}

		const winnersNames = chain(game.players)
			.filter(p => p.placement === 1)
			.map(p => p.username)
			.value();
		const message = `Game over. ${winnersNames.join(", ")} won!`;
		this.statusMessage = message;
	}

	setActiveView(view: ActiveView) {
		this.activeView = view;
	}

	setStatusMessage(message: string) {
		this.activeView = ActiveView.Map;
		this.statusMessage = message;
		this.availableCommands = [];
		this.interactiveElements = [];
	}

	setStatusFromWorkflow(state: WorkflowState) {
		this.statusMessage = state.message;
		this.availableCommands = state.commands ?? [];
		this.interactiveElements = state.interactiveElements ?? [];
		!isNil(state.view) && (this.activeView = state.view);
	}

	setWaitingForAction(actions?: AvailableActionDto[] | undefined) {
		this.statusMessage = "You should take an action";
		this.availableCommands = [];
		this.interactiveElements = [];
		this.activeView = ActiveView.Map;
		!isNil(actions) && (this.availableActions = actions);
	}

	setConnectedToGame(gameId: string) {
		this.hubState.connectionState = HubConnectionState.Connected;
		this.hubState.connectedToGameId = gameId;
	}

	setConnecting() {
		this.hubState.connectionState = HubConnectionState.Connecting;
		this.hubState.connectedToGameId = null;
	}

	setDisconnected() {
		this.hubState.connectionState = HubConnectionState.Disconnected;
		this.hubState.connectedToGameId = null;
	}

	setDisconnecting() {
		this.hubState.connectionState = HubConnectionState.Disconnecting;
		this.hubState.connectedToGameId = null;
	}

	userJoined(userId: string) {
		this.hubState.onlineUsers.push(userId);
	}

	userLeft(userId: string) {
		this.hubState.onlineUsers = without(this.hubState.onlineUsers, userId);
	}

	setOnlineUsers(userIds: string[]) {
		this.hubState.onlineUsers = userIds;
	}

	saveNotesFeedbackDisplayed() {
		this.saveNotesProgress = "idle";
	}

	rotateSector(id: string, rotation: number) {
		if (!this.gameState) {
			throw new Error("Can't rotate sector when there's not game...");
		}

		const sector = this.gameState.boardState.map.sectors.find(s => s.id === id)!;
		sector.rotation = rotation;
	}

	resetSectors(map: MapDto) {
		if (!this.gameState) {
			throw new Error("Can't rotate sector when there's not game...");
		}

		this.gameState.boardState.map = map;
	}

	//#region Api calls

	async fetchActiveGame(id: string) {
		this.status = "loading";
		try {
			const gameState = await this.httpClient.get<GameStateDto>(`api/GaiaProject/GetGame/${id}`);
			if (isNil(gameState)) {
				throw new Error("Not found!!");
			}

			runInAction(() => {
				this.gameState = gameState;
				this.status = "success";
			});
		} catch (err) {
			runInAction(() => {
				this.gameState = null;
				this.status = "failure";
			});
		}
	}

	async executePlayerAction(gameId: string, action: ActionDto) {
		this.actionProgress = "loading";
		try {
			const result = await this.httpClient.post<ExecuteActionResult>(`api/GaiaProject/Action/${gameId}`, action);
			if (!result || !result.handled) {
				throw new Error(result.errorMessage);
			}
		} catch (err: any) {
			this.handleActionFailure(err.message);
		}
	}

	async rollbackGameAtAction(gameId: string, actionId: number) {
		this.rollbackProgress = "loading";
		try {
			await this.httpClient.get(`api/GaiaProject/RollbackGameAtAction/${gameId}?actionId=${actionId}`);
			runInAction(() => {
				this.rollbackProgress = "success";
			});
		} catch (err: any) {
			runInAction(() => {
				this.rollbackProgress = "failure";
				this.statusMessage = "Rollback failed";
			});
		}
	}

	async fetchNotes(gameId: string) {
		try {
			const notes = await this.httpClient.get<string>(`api/GaiaProject/GetNotes/${gameId}`, { readAsString: true });
			runInAction(() => {
				this.playerNotes = notes;
			});
		} catch (err) {
			runInAction(() => {
				this.playerNotes = "";
			});
		}
	}

	async saveNotes(gameId: string, notes: Nullable<string>) {
		this.saveNotesProgress = "loading";
		try {
			await this.httpClient.put(`api/GaiaProject/SaveNotes/${gameId}`, { notes });
			runInAction(() => {
				this.saveNotesProgress = "success";
			});
		} catch (err) {
			runInAction(() => {
				this.saveNotesProgress = "failure";
			});
		}
	}

	private handleActionFailure(msg: string) {
		this.actionProgress = "idle";
		this.statusMessage = msg;
	}

	//#endregion
}
