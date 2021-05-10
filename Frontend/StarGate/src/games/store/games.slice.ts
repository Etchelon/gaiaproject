import { createAsyncThunk, createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { GameInfoDto } from "../../dto/interfaces";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { GamesSliceState } from "./types";

const gamesAdapter = createEntityAdapter<GameInfoDto>({
	sortComparer(lhs, rhs) {
		return rhs.created.localeCompare(lhs.created);
	},
});

const initialState: GamesSliceState = gamesAdapter.getInitialState({
	status: "idle",
	deleteGameProgress: "idle",
	lastFetched: null,
	lastFetchParams: null,
	error: null,
});

export type GameKind = "waiting" | "active" | "finished";

export const fetchGames = createAsyncThunk("games/fetch", async (kind: GameKind) => ({
	data: await httpClient.get<GameInfoDto[]>(`api/GaiaProject/GetUserGames?kind=${kind}`),
	params: JSON.stringify({ kind }),
}));

interface DeleteGameActionPayload {
	gameId: string;
}

export const deleteGameAction = createAsyncThunk("activeGame/deleteGameAction", async ({ gameId }: DeleteGameActionPayload) => {
	await httpClient.delete(`api/GaiaProject/DeleteGame/${gameId}`);
});

const gamesSlice = createSlice({
	name: "games",
	initialState,
	reducers: {},
	extraReducers: {
		[fetchGames.pending.type]: state => {
			state.status = "loading";
		},
		[fetchGames.fulfilled.type]: (state, action: PayloadAction<{ data: GameInfoDto[]; params: string }>) => {
			state.status = "success";
			state.lastFetched = new Date().toISOString();
			state.lastFetchParams = action.payload.params;
			gamesAdapter.setAll(state, action.payload.data);
		},
		[fetchGames.rejected.type]: state => {
			state.status = "failure";
			gamesAdapter.removeAll(state);
		},
		[deleteGameAction.pending.type]: state => {
			state.deleteGameProgress = "loading";
		},
		[deleteGameAction.fulfilled.type]: state => {
			state.deleteGameProgress = "success";
		},
		[deleteGameAction.rejected.type]: state => {
			state.deleteGameProgress = "failure";
		},
	},
});

export default gamesSlice.reducer;

//#region Selectors
export const selectDeleteGameProgress = (state: AppStore) => state.games.deleteGameProgress;
export const selectGamesStatus = (state: AppStore) => state.games.status;
export const selectLastFetchInfo = (state: AppStore) => ({ when: state.games.lastFetched, params: state.games.lastFetchParams });
export const { selectAll: selectGames, selectIds: selectGamesIds, selectById: selectGame } = gamesAdapter.getSelectors((state: AppStore) => state.games as GamesSliceState);
//#endregion
