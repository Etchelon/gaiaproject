import { createAsyncThunk } from "@reduxjs/toolkit";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { Nullable } from "../../utils/miscellanea";

interface SaveNotesPayload {
	gameId: string;
	notes: Nullable<string>;
}

export const fetchNotes = createAsyncThunk("activeGame/fetchPlayerNotes", async (gameId: string) => {
	return await httpClient.get<string>(`api/GaiaProject/GetNotes/${gameId}`, { readAsString: true });
});

export const saveNotes = createAsyncThunk("activeGame/savePlayerNotes", async ({ gameId, notes }: SaveNotesPayload) => {
	await httpClient.put(`api/GaiaProject/SaveNotes/${gameId}`, { notes });
	return notes;
});

export const selectPlayerNotes = (state: AppStore) => state.activeGame.playerNotes ?? "";
export const selectSaveNotesProgress = (state: AppStore) => state.activeGame.saveNotesProgress;
