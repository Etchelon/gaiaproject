import { configureStore } from "@reduxjs/toolkit";
import activeUserReducer from "../frame/store/active-user.slice";
import activeGameReducer from "../game-page/store/active-game.slice";
import gamesReducer from "../games/store/games.slice";
import { AppStore } from "./types";

export default configureStore<AppStore>({
	reducer: {
		games: gamesReducer,
		activeGame: activeGameReducer,
		activeUser: activeUserReducer,
	},
});
