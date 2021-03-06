import { HubConnectionState } from "@microsoft/signalr";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { AdvancedTechnologyTileType, FederationTokenType, ResearchTrackType, RoundBoosterType, StandardTechnologyTileType } from "../../dto/enums";
import { ActionDto, AvailableActionDto, GameStateDto } from "../../dto/interfaces";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { Identifier, isLastRound, Nullable } from "../../utils/miscellanea";
import { InteractiveElementState, InteractiveElementType } from "../workflows/enums";
import { ActiveView, WorkflowState } from "../workflows/types";
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
};

export const fetchActiveGame = createAsyncThunk("activeGame/fetch", async (id: string, { rejectWithValue }) => {
	const gameState = await httpClient.get<GameStateDto>(`api/GaiaProject/GetGame/${id}`);
	if (_.isNil(gameState)) {
		return rejectWithValue("Not found!!");
	}
	return gameState;
});

interface ExecuteActionPayload {
	gameId: string;
	action: ActionDto;
}

interface ExecuteActionResult {
	handled: boolean;
	errorMessage: Nullable<string>;
}

export const executePlayerAction = createAsyncThunk("activeGame/executePlayerAction", async ({ gameId, action }: ExecuteActionPayload, { rejectWithValue }) => {
	const result = await httpClient.post<ExecuteActionResult>(`api/GaiaProject/Action/${gameId}`, action);
	if (!result?.handled) {
		return rejectWithValue(result?.errorMessage);
	}
});

interface RollbackGameAtActionPayload {
	gameId: string;
	actionId: number;
}

export const rollbackGameAtAction = createAsyncThunk("activeGame/rollbackGameAtAction", async ({ gameId, actionId }: RollbackGameAtActionPayload) => {
	await httpClient.get(`api/GaiaProject/RollbackGameAtAction/${gameId}?actionId=${actionId}`);
});

const activeGameSlice = createSlice({
	name: "activeGame",
	initialState,
	reducers: {
		setCurrentUser: (state, action: PayloadAction<string>) => {
			state.currentUserId = action.payload;
		},
		clear: state => {
			state = {
				...initialState,
				currentUserId: state.currentUserId,
			};
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
		// Omit to set the actionProgress to idle upon success: the async state change from SignalR might take a while
		// and the StatusBar would be in an inconsistent state until then. Better leave it "loading"
		/* [executePlayerAction.fulfilled.type]: state => {
			state.actionProgress = "idle";
		},*/
		[executePlayerAction.rejected.type]: (state, action: any) => {
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
} = activeGameSlice.actions;

export const selectActiveGame = (state: AppStore) => state.activeGame.gameState;
export const selectActiveGameStatus = (state: AppStore) => state.activeGame.status;
export const selectRollbackProgress = (state: AppStore) => state.activeGame.rollbackProgress;
export const selectActiveView = (state: AppStore) => state.activeGame.activeView;
export const selectCurrentPlayer = (state: AppStore) => _.find(state.activeGame.gameState?.players, p => p.id === state.activeGame.currentUserId) ?? null;
export const selectActivePlayer = (state: AppStore) => state.activeGame.gameState?.activePlayer;
export const selectPlayers = (state: AppStore) => {
	const game = selectActiveGame(state);
	const currentPlayer = selectCurrentPlayer(state);
	if (_.isNil(game) || _.isNil(currentPlayer)) {
		return [];
	}

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
	const currentPlayer = selectCurrentPlayer(state);
	if (_.isNil(game) || _.isNil(currentPlayer)) {
		return [];
	}

	const available = game.boardState.availableRoundBoosters;
	const playersBoosters = _.chain(game.players)
		.filter(p => !_.isNil(p.state?.roundBooster))
		.sortBy(p => (p.id === currentPlayer.id ? 0 : 1))
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
export const selectIsExecutingAction = (state: AppStore) => state.activeGame.actionProgress === "loading";
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
