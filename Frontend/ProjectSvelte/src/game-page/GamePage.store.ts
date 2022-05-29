import type { GameStateDto } from "$dto/interfaces";
import { ActiveView } from "$utils/types";
import { writable } from "svelte/store";

export interface IGamePageStore {
	game: GameStateDto;
	activeView: ActiveView;
}

export const gamePageStore = writable<IGamePageStore>({} as any, set => {
	import("../data").then((mod: any) => {
		console.log({ mod });
		set({
			game: mod.gameDto,
			activeView: ActiveView.Map,
		});
	});
});
