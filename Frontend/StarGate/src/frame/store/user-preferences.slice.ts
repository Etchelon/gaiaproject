import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { AppStore } from "../../store/types";
import { UserPreferencesState } from "./types";

const initialState: UserPreferencesState = {
	drawerState: "open",
};

const userPreferencesSlice = createSlice({
	name: "userPreferences",
	initialState,
	reducers: {
		loadUserPreferences: (state, action: PayloadAction<UserPreferencesState>) => {
			state.drawerState = action.payload.drawerState;
		},
		setDrawerState: (state, action: PayloadAction<"open" | "close">) => {
			state.drawerState = action.payload;
		},
	},
});

export default userPreferencesSlice.reducer;

export const { loadUserPreferences, setDrawerState } = userPreferencesSlice.actions;
export const selectUserPreferences = (state: AppStore) => state.userPreferences;
export const selectIsDrawerOpen = (state: AppStore) => state.userPreferences.drawerState === "open";
