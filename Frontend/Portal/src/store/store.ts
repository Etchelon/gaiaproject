import { configureStore } from "@reduxjs/toolkit";
import activeGameReducer from "../game-page/store/active-game.slice";
import { AppStore } from "./types";

export default configureStore<AppStore>({
	reducer: {
		activeGame: activeGameReducer,
	},
});
