import { UserPreferencesState } from "../frame/store/types";
import { ActiveGameSliceState } from "../game-page/store/types";
import { GamesSliceState } from "../games/store/types";

export interface AppStore {
	games: GamesSliceState;
	activeGame: ActiveGameSliceState;
	userPreferences: UserPreferencesState;
}
