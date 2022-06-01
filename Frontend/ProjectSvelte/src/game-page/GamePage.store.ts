import type { GameStateDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import { ActiveView } from "$utils/types";
import { isNil } from "lodash";
import { Writable, writable } from "svelte/store";

export interface IGamePageStore {
	game: GameStateDto;
	activeView: ActiveView;
}

export const getGamePageStore = (gameId: string, http: HttpClient): Writable<IGamePageStore> => {
	console.log("Creating game page store");

	const { set, subscribe, update } = writable<IGamePageStore>({ game: null, activeView: ActiveView.Map } as any);
	http.get<GameStateDto>(`api/GaiaProject/GetGame/${gameId}`)
		.then(gameState => {
			if (isNil(gameState)) {
				throw new Error("Not found!!");
			}

			set({
				game: gameState,
				activeView: ActiveView.Map,
			});
		})
		.catch(err => {
			set({
				game: null as any,
				activeView: ActiveView.Map,
			});
		});

	return {
		set,
		subscribe,
		update,
	};
};
