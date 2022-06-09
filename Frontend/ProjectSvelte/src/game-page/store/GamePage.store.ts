import type { ActionDto, AvailableActionDto, GameStateDto, MapDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import { asyncDelay, isLastRound, Nullable } from "$utils/miscellanea";
import { ActiveView, LoadingStatus } from "$utils/types";
import { HubConnectionState } from "@microsoft/signalr";
import { chain, cloneDeep, every, findIndex, isNil, partition, sortBy, without } from "lodash";
import { derived, get, Writable, writable } from "svelte/store";
import type { Command, InteractiveElement, WorkflowState } from "../workflows/types";

interface IHubState {
	connectionState: HubConnectionState;
	connectedToGameId: Nullable<string>;
	onlineUsers: string[];
}

interface ExecuteActionResult {
	handled: boolean;
	errorMessage?: string;
}

interface IGamePageStore {
	currentUserId: Writable<Nullable<string>>;
	game: Writable<Nullable<GameStateDto>>;
	statusMessage: Writable<string>;
	activeView: Writable<ActiveView>;
	availableCommands: Writable<Command[]>;
	interactiveElements: Writable<InteractiveElement[]>;
	availableActions: Writable<AvailableActionDto[]>;
	actionProgress: Writable<LoadingStatus>;
	rollbackProgress: Writable<LoadingStatus>;
	hubState: Writable<IHubState>;
	playerNotes: Writable<Nullable<string>>;
	saveNotesProgress: Writable<LoadingStatus>;
	playerSettings?: Writable<any>;
}

export class GamePageStore implements IGamePageStore {
	currentUserId = writable<Nullable<string>>(null);
	game = writable<GameStateDto>();
	statusMessage = writable("");
	activeView = writable(ActiveView.Map);
	availableCommands = writable<Command[]>([]);
	interactiveElements = writable<InteractiveElement[]>([]);
	availableActions = writable<AvailableActionDto[]>([]);
	actionProgress = writable<LoadingStatus>("idle");
	rollbackProgress = writable<LoadingStatus>("idle");
	hubState = writable<IHubState>({
		connectionState: HubConnectionState.Disconnected,
		connectedToGameId: null,
		onlineUsers: [],
	});
	playerNotes = writable<Nullable<string>>(null);
	saveNotesProgress = writable<LoadingStatus>("idle");
	playerSettings?: Writable<any> | undefined;

	//#region Derived stores

	currentPlayer = derived([this.game, this.currentUserId], ([$game, $uId]) => $game?.players.find(p => p.id === $uId));
	activePlayer = derived(this.game, $game => $game?.activePlayer);
	isSpectator = derived([this.game, this.currentPlayer], ([$game, $currentPlayer]) => $game !== null && $currentPlayer === null);
	players = derived([this.game, this.currentPlayer, this.isSpectator], ([$game, $currentPlayer, $isSpectator]) => {
		if (isNil($game)) {
			return [];
		}

		if ($isSpectator) {
			return $game.players;
		}

		const currentPlayerIndex = findIndex($game.players, p => p.id === $currentPlayer?.id);
		if (currentPlayerIndex === 0) {
			return $game.players;
		}

		// Until all players have selected a race and have the state object, keep the initial order
		const allPlayersAreSetup = every($game.players, p => !isNil(p.state));
		if (!allPlayersAreSetup) {
			return $game.players;
		}

		// Assume players are already sorted by turn order in the current round
		const allPlayers = sortBy($game.players, p => p.state.currentRoundTurnOrder);
		const [active, havePassed] = partition(allPlayers, p => $game.players.indexOf(p) < currentPlayerIndex);
		const playerHasPassed = $currentPlayer?.state.hasPassed ? havePassed : active;
		const playersToPartition = playerHasPassed ? havePassed : active;
		const [before, fromCurrentPlayer] = partition(playersToPartition, p => $game.players.indexOf(p) < currentPlayerIndex);
		return [...fromCurrentPlayer, ...before, ...(playerHasPassed ? active : havePassed)];
	});
	sortedPlayers = derived(this.players, $players => sortBy($players, (p, index) => p.state?.currentRoundTurnOrder ?? index));
	sortedActivePlayers = derived(this.players, $players =>
		chain($players)
			.filter(p => !p.state?.hasPassed)
			.sortBy((p, index) => p.state?.currentRoundTurnOrder ?? index)
			.value()
	);
	sortedPassedPlayers = derived([this.game, this.players], ([$game, $players]) => {
		const isLastRound_ = isLastRound($game);
		return chain($players)
			.filter(p => p.state?.hasPassed ?? false)
			.sortBy((p, index) => (isLastRound_ ? p.state?.currentRoundTurnOrder : p.state?.nextRoundTurnOrder) ?? index)
			.value();
	});
	allRoundBoosters = derived([this.game, this.currentPlayer], ([$game, $currentPlayer]) => {
		if (isNil($game)) {
			return [];
		}

		const available = $game.boardState.availableRoundBoosters;
		const playersBoosters = chain($game.players)
			.filter(p => !isNil(p.state?.roundBooster))
			.sortBy(p => (p.id === $currentPlayer?.id ? 0 : 1))
			.map(p => ({
				id: p.state.roundBooster.id,
				isTaken: true,
				player: {
					id: p.id,
					username: p.username,
					avatarUrl: "",
					raceId: p.raceId,
					raceName: null,
					color: null,
					points: p.state?.points,
					isActive: p.isActive,
					placement: null,
				},
				used: p.state.roundBooster.used,
			}))
			.value();
		return [...playersBoosters, ...available];
	});

	//#endregion

	constructor(private readonly http: HttpClient) {
		console.log("Creating game page store");
	}

	//#region Api calls

	loadGame = async (id: string) => {
		const gameState = await this.http.get<GameStateDto>(`api/GaiaProject/GetGame/${id}`);
		if (isNil(gameState)) {
			throw new Error("Not found!!");
		}

		this.game.set(gameState);
		return gameState.name;
	};

	executePlayerAction = async (gameId: string, action: ActionDto) => {
		this.actionProgress.set("loading");
		try {
			const result = await this.http.post<ExecuteActionResult>(`api/GaiaProject/Action/${gameId}`, action);
			if (!result || !result.handled) {
				throw new Error(result.errorMessage);
			}
		} catch (err: any) {
			this.handleActionFailure(err.message);
		}
	};

	async rollbackGameAtAction(gameId: string, actionId: number) {
		this.rollbackProgress.set("loading");
		try {
			await this.http.get(`api/GaiaProject/RollbackGameAtAction/${gameId}?actionId=${actionId}`);
			this.rollbackProgress.set("success");
		} catch (err: any) {
			this.rollbackProgress.set("failure");
			this.statusMessage.set("Rollback failed");
		}
	}

	async fetchNotes(gameId: string) {
		try {
			const notes = await this.http.get<string>(`api/GaiaProject/GetNotes/${gameId}`, { readAsString: true });
			this.playerNotes.set(notes);
		} catch (err) {
			this.playerNotes.set("");
		}
	}

	async saveNotes(gameId: string, notes: Nullable<string>) {
		this.saveNotesProgress.set("loading");
		try {
			await this.http.put(`api/GaiaProject/SaveNotes/${gameId}`, { notes });
			this.saveNotesProgress.set("success");
		} catch (err) {
			this.saveNotesProgress.set("failure");
		}
	}

	private handleActionFailure(msg: string) {
		this.actionProgress.set("idle");
		this.statusMessage.set(msg);
	}

	//#endregion

	setCurrentUser = (id: string) => {
		this.currentUserId.set(id);
	};

	clearStatus = () => {
		this.activeView.set(ActiveView.Map);
		this.statusMessage.set("");
		this.availableCommands.set([]);
	};

	gameUpdated = (game: GameStateDto) => {
		this.actionProgress.set("idle");
		this.rollbackProgress.set("idle");
		this.game.set(game);
		const gameIsOver = !isNil(game.ended);
		if (!gameIsOver) {
			return;
		}

		const winnersNames = chain(game.players)
			.filter(p => p.placement === 1)
			.map(p => p.username)
			.value();
		const message = `Game over. ${winnersNames.join(", ")} won!`;
		this.statusMessage.set(message);
	};

	setActiveView = (view: ActiveView) => {
		this.activeView.set(view);
	};

	setStatusMessage = (message: string) => {
		this.activeView.set(ActiveView.Map);
		this.statusMessage.set(message);
		this.availableCommands.set([]);
		this.interactiveElements.set([]);
	};

	setStatusFromWorkflow = (state: WorkflowState) => {
		this.statusMessage.set(state.message);
		this.availableCommands.set(state.commands ?? []);
		this.interactiveElements.set(state.interactiveElements ?? []);
		!isNil(state.view) && this.activeView.set(state.view);
	};

	setWaitingForAction = (actions?: AvailableActionDto[] | undefined) => {
		this.statusMessage.set("You should take an action");
		this.availableCommands.set([]);
		this.interactiveElements.set([]);
		this.activeView.set(ActiveView.Map);
		!isNil(actions) && this.availableActions.set(actions);
	};

	updateGameState = (newGameState: GameStateDto) => {
		this.game.set(newGameState);
	};

	saveNotesFeedbackDisplayed = () => {
		this.saveNotesProgress.set("idle");
	};

	rotateSector = (id: string, rotation: number) => {
		const game = this.assertGame();
		const index = game.boardState.map.sectors.findIndex(s => s.id === id);
		if (index === -1) {
			throw new Error(`Sector with id ${id} not found.`);
		}

		const sector = game.boardState.map.sectors[index];
		sector.rotation = rotation;
		const newState = cloneDeep(game);
		newState.boardState.map.sectors.splice(index, 1, sector);
		this.game.set(newState);
	};

	resetSectors = (map: MapDto) => {
		const game = this.assertGame();
		const newState = cloneDeep(game);
		game.boardState.map = map;
		this.game.set(newState);
	};

	private assertGame = () => {
		const game = get(this.game);
		if (!game) {
			throw new Error("Game not initialized.");
		}
		return game;
	};

	//#region SignalR

	userJoined = (userId: string) => {
		this.hubState.update(state => {
			const onlineUsers = [...state.onlineUsers, userId];
			return {
				...state,
				onlineUsers,
			};
		});
	};

	setConnectedToGame = (gameId: string) => {
		this.hubState.update(state => ({
			...state,
			connectionState: HubConnectionState.Connected,
			connectedToGameId: gameId,
		}));
	};

	setConnecting = () => {
		this.hubState.update(state => ({
			...state,
			connectionState: HubConnectionState.Connecting,
			connectedToGameId: null,
		}));
	};

	setDisconnected = () => {
		this.hubState.update(state => ({
			...state,
			connectionState: HubConnectionState.Disconnected,
			connectedToGameId: null,
		}));
	};

	setDisconnecting = () => {
		this.hubState.update(state => ({
			...state,
			connectionState: HubConnectionState.Disconnecting,
			connectedToGameId: null,
		}));
	};

	userLeft = (userId: string) => {
		this.hubState.update(state => {
			const onlineUsers = without(state.onlineUsers, userId);
			return {
				...state,
				onlineUsers,
			};
		});
	};

	setOnlineUsers = (userIds: string[]) => {
		this.hubState.update(state => ({
			...state,
			onlineUsers: userIds,
		}));
	};

	//#endregion
}
