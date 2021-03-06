import { HubConnectionState } from "@microsoft/signalr";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { AdvancedTechnologyTileType, FederationTokenType, ResearchTrackType, RoundBoosterType, StandardTechnologyTileType } from "../../dto/enums";
import { AvailableActionDto, GameStateDto, MapDto } from "../../dto/interfaces";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { Identifier, isLastRound } from "../../utils/miscellanea";
import { InteractiveElementState, InteractiveElementType } from "../workflows/enums";
import { ActiveView, WorkflowState } from "../workflows/types";
import { executePlayerAction, rollbackGameAtAction } from "./actions-thunks";
import { fetchNotes, saveNotes } from "./notes-thunks";
import { ActiveGameSliceState } from "./types";

const initialState: ActiveGameSliceState = {
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

export const fetchActiveGame = createAsyncThunk("activeGame/fetch", async (id: string, { rejectWithValue }) => {
	const gameState = await httpClient.get<GameStateDto>(`api/GaiaProject/GetGame/${id}`);
	if (_.isNil(gameState)) {
		return rejectWithValue("Not found!!");
	}
	return gameState;
});

const activeGameSlice = createSlice({
	name: "activeGame",
	initialState,
	reducers: {
		setCurrentUser: (state, action: PayloadAction<string>) => {
			state.currentUserId = action.payload;
		},
		clear: state => {
			const currentUserId = state.currentUserId;
			_.assign(state, initialState);
			state.currentUserId = currentUserId;
		},
		clearStatus: state => {
			state.activeView = ActiveView.Map;
			state.statusMessage = "";
			state.availableCommands = [];
		},
		gameUpdated: (state, action: PayloadAction<GameStateDto>) => {
			state.actionProgress = "idle";
			state.rollbackProgress = "idle";
			const game = action.payload;
			state.gameState = game;
			const gameIsOver = !_.isNil(game.ended);
			if (!gameIsOver) {
				return;
			}

			const winnersNames = _.chain(game.players)
				.filter(p => p.placement === 1)
				.map(p => p.username)
				.value();
			const message = `Game over. ${winnersNames.join(", ")} won!`;
			state.statusMessage = message;
		},
		setActiveView: (state, action: PayloadAction<ActiveView>) => {
			state.activeView = action.payload;
		},
		setStatusMessage: (state, action: PayloadAction<string>) => {
			state.activeView = ActiveView.Map;
			state.statusMessage = action.payload;
			state.availableCommands = [];
			state.interactiveElements = [];
		},
		setStatusFromWorkflow: (state, action: PayloadAction<WorkflowState>) => {
			state.statusMessage = action.payload.message;
			state.availableCommands = action.payload.commands ?? [];
			state.interactiveElements = action.payload.interactiveElements ?? [];
			!_.isNil(action.payload.view) && (state.activeView = action.payload.view);
		},
		setWaitingForAction: (state, action: PayloadAction<AvailableActionDto[] | undefined>) => {
			state.statusMessage = "You should take an action";
			state.availableCommands = [];
			state.interactiveElements = [];
			state.activeView = ActiveView.Map;
			!_.isNil(action.payload) && (state.availableActions = action.payload);
		},
		setConnectedToGame: (state, action: PayloadAction<string>) => {
			state.hubState.connectionState = HubConnectionState.Connected;
			state.hubState.connectedToGameId = action.payload;
		},
		setConnecting: state => {
			state.hubState.connectionState = HubConnectionState.Connecting;
			state.hubState.connectedToGameId = null;
		},
		setDisconnected: state => {
			state.hubState.connectionState = HubConnectionState.Disconnected;
			state.hubState.connectedToGameId = null;
		},
		setDisconnecting: state => {
			state.hubState.connectionState = HubConnectionState.Disconnecting;
			state.hubState.connectedToGameId = null;
		},
		userJoined: (state, action: PayloadAction<string>) => {
			const userId = action.payload;
			state.hubState.onlineUsers.push(userId);
		},
		userLeft: (state, action: PayloadAction<string>) => {
			const userId = action.payload;
			_.pull(state.hubState.onlineUsers, userId);
		},
		setOnlineUsers: (state, action: PayloadAction<string[]>) => {
			const userIds = action.payload;
			state.hubState.onlineUsers = userIds;
		},
		saveNotesFeedbackDisplayed: state => {
			state.saveNotesProgress = "idle";
		},
		rotateSector: (state, action: PayloadAction<{ id: string; rotation: number }>) => {
			const { id, rotation } = action.payload;
			const sector = _.find(state.gameState!.boardState.map.sectors, s => s.id === id)!;
			sector.rotation = rotation;
		},
		resetSectors: (state, action: PayloadAction<MapDto>) => {
			state.gameState!.boardState.map = action.payload;
		},
	},
	extraReducers: {
		[fetchActiveGame.pending.type]: state => {
			state.status = "loading";
		},
		[fetchActiveGame.fulfilled.type]: (state, action: PayloadAction<GameStateDto>) => {
			state.status = "success";
			state.gameState = action.payload;
		},
		[fetchActiveGame.rejected.type]: state => {
			state.status = "failure";
			state.gameState = null;
		},
		[executePlayerAction.pending.type]: state => {
			state.actionProgress = "loading";
		},
		// [executePlayerAction.fulfilled.type]: state => {
		// 	state.actionProgress = "success";
		// },
		[executePlayerAction.rejected.type]: (state, action: PayloadAction<string>) => {
			state.actionProgress = "idle";
			state.statusMessage = action.payload;
		},
		[rollbackGameAtAction.pending.type]: state => {
			state.rollbackProgress = "loading";
		},
		[rollbackGameAtAction.fulfilled.type]: state => {
			state.rollbackProgress = "success";
		},
		[rollbackGameAtAction.rejected.type]: state => {
			state.rollbackProgress = "failure";
			state.statusMessage = "Rollback failed";
		},
		[fetchNotes.fulfilled.type]: (state, action: PayloadAction<string>) => {
			state.playerNotes = action.payload ?? "";
		},
		[saveNotes.pending.type]: state => {
			state.saveNotesProgress = "loading";
		},
		[saveNotes.fulfilled.type]: (state, action: PayloadAction<string>) => {
			state.saveNotesProgress = "success";
			state.playerNotes = action.payload;
		},
		[saveNotes.rejected.type]: state => {
			state.saveNotesProgress = "failure";
		},
	},
});

export default activeGameSlice.reducer;

export const {
	setCurrentUser,
	clear: unloadActiveGame,
	clearStatus,
	gameUpdated,
	setActiveView,
	setStatusMessage,
	setStatusFromWorkflow,
	setWaitingForAction,
	setConnectedToGame,
	setConnecting,
	setDisconnected,
	setDisconnecting,
	userJoined,
	userLeft,
	setOnlineUsers,
	saveNotesFeedbackDisplayed,
	rotateSector,
	resetSectors,
} = activeGameSlice.actions;

//#region Selectors

export const selectActiveGame = (state: AppStore) => state.activeGame.gameState;
export const selectActiveGameStatus = (state: AppStore) => state.activeGame.status;
export const selectActiveView = (state: AppStore) => state.activeGame.activeView;
export const selectCurrentPlayer = (state: AppStore) => _.find(state.activeGame.gameState?.players, p => p.id === state.activeGame.currentUserId) ?? null;
export const selectActivePlayer = (state: AppStore) => state.activeGame.gameState?.activePlayer;
export const selectIsSpectator = (state: AppStore) => selectActiveGame(state) !== null && selectCurrentPlayer(state) === null;
export const selectPlayers = (state: AppStore) => {
	const game = selectActiveGame(state);
	if (_.isNil(game)) {
		return [];
	}

	const isSpectator = selectIsSpectator(state);
	if (isSpectator) {
		return game.players;
	}

	const currentPlayer = selectCurrentPlayer(state)!;
	const currentPlayerIndex = _.findIndex(game.players, p => p.id === currentPlayer.id);
	if (currentPlayerIndex === 0) {
		return game.players;
	}

	// Until all players have selected a race and have the state object, keep the initial order
	const allPlayersAreSetup = _.every(game.players, p => !_.isNil(p.state));
	if (!allPlayersAreSetup) {
		return game.players;
	}

	// Assume players are already sorted by turn order in the current round
	const allPlayers = _.sortBy(game.players, p => p.state.currentRoundTurnOrder);
	const [active, havePassed] = _.partition(allPlayers, p => _.indexOf(game.players, p) < currentPlayerIndex);
	const playerHasPassed = currentPlayer.state.hasPassed ? havePassed : active;
	const playersToPartition = playerHasPassed ? havePassed : active;
	const [before, fromCurrentPlayer] = _.partition(playersToPartition, p => _.indexOf(game.players, p) < currentPlayerIndex);
	return [...fromCurrentPlayer, ...before, ...(playerHasPassed ? active : havePassed)];
};
export const selectSortedPlayers = (state: AppStore) => {
	const players = selectPlayers(state);
	return _.sortBy(players, (p, index) => p.state?.currentRoundTurnOrder ?? index);
};
export const selectSortedActivePlayers = (state: AppStore) => {
	const players = selectPlayers(state);
	return _.chain(players)
		.filter(p => !p.state?.hasPassed)
		.sortBy((p, index) => p.state?.currentRoundTurnOrder ?? index)
		.value();
};
export const selectSortedPassedPlayers = (state: AppStore) => {
	const players = selectPlayers(state);
	const isLastRound_ = isLastRound(state.activeGame.gameState!);
	return _.chain(players)
		.filter(p => p.state?.hasPassed ?? false)
		.sortBy((p, index) => (isLastRound_ ? p.state?.currentRoundTurnOrder : p.state?.nextRoundTurnOrder) ?? index)
		.value();
};
export const selectAllRoundBoosters = (state: AppStore) => {
	const game = selectActiveGame(state);
	if (_.isNil(game)) {
		return [];
	}

	const currentPlayer = selectCurrentPlayer(state);
	const available = game.boardState.availableRoundBoosters;
	const playersBoosters = _.chain(game.players)
		.filter(p => !_.isNil(p.state?.roundBooster))
		.sortBy(p => (p.id === currentPlayer?.id ? 0 : 1))
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
};
export const selectStatusMessage = (state: AppStore) => state.activeGame.statusMessage;
export const selectAvailableCommands = (state: AppStore) => state.activeGame.availableCommands;
export const selectAvailableActions = (state: AppStore) => state.activeGame.availableActions;

const selectInteractionState = (type: InteractiveElementType) => (id: Identifier) => (state: AppStore, playerId?: string) => {
	if (!_.isNil(playerId) && playerId !== state.activeGame.currentUserId) {
		return { isClickable: false, isSelected: false };
	}
	const interactiveElement = _.find(state.activeGame.interactiveElements, el => el.type === type && el.id === id);
	const interactionState = interactiveElement?.state ?? InteractiveElementState.Disabled;
	const isClickable = interactionState === InteractiveElementState.Enabled;
	const isSelected = interactionState === InteractiveElementState.Selected;
	return { isClickable, isSelected, notes: interactiveElement?.notes };
};
export const selectHexInteractionState = (hexId: string) => selectInteractionState(InteractiveElementType.Hex)(hexId);
export const selectAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) => selectInteractionState(InteractiveElementType.AdvancedTile)(type);
export const selectStandardTileInteractionState = (type: StandardTechnologyTileType) => selectInteractionState(InteractiveElementType.StandardTile)(type);
export const selectOwnStandardTileInteractionState = (type: StandardTechnologyTileType) => selectInteractionState(InteractiveElementType.OwnStandardTile)(type);
export const selectOwnAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) => selectInteractionState(InteractiveElementType.OwnAdvancedTile)(type);
export const selectResearchTrackInteractionState = (type: ResearchTrackType) => selectInteractionState(InteractiveElementType.ResearchStep)(type);
export const selectActionSpaceInteractionState = (elementType: InteractiveElementType) => (type: number) => selectInteractionState(elementType)(type);
export const selectRoundBoosterInteractionState = (elementType: InteractiveElementType) => (type: RoundBoosterType) => selectInteractionState(elementType)(type);
export const selectFederationTokenStackInteractionState = (token: FederationTokenType) => selectInteractionState(InteractiveElementType.FederationToken)(token);
export const selectOwnFederationTokenInteractionState = (type: FederationTokenType) => selectInteractionState(InteractiveElementType.OwnFederationToken)(type);
export const selectIsOnline = (playerId: string) => (state: AppStore) => {
	const isCurrentPlayer = playerId === state.activeGame.currentUserId;
	return isCurrentPlayer ? state.activeGame.hubState.connectionState === HubConnectionState.Connected : _.includes(state.activeGame.hubState.onlineUsers, playerId);
};

//#endregion
