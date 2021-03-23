import { createAsyncThunk } from "@reduxjs/toolkit";
import { ActionDto } from "../../dto/interfaces";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { Nullable } from "../../utils/miscellanea";

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

//#region Selectors

export const selectIsExecutingAction = (state: AppStore) => state.activeGame.actionProgress === "loading";
export const selectHasExecutedAction = (state: AppStore) => state.activeGame.actionProgress === "success";
export const selectRollbackProgress = (state: AppStore) => state.activeGame.rollbackProgress;

//#endregion
