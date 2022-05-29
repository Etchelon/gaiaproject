import { getContext, setContext } from "svelte";
import type { Writable } from "svelte/store";
import { gamePageStore, IGamePageStore } from "./GamePage.store";

const key = Symbol.for("GamePageContext");

interface IGamePageContext {
	id: string;
	activeWorkflow: any;
	store: Writable<IGamePageStore>;
}

export const getGamePageContext = () => getContext<IGamePageContext>(key);

export const setGamePageContext = (id: string) => {
	const ctx: IGamePageContext = {
		id,
		activeWorkflow: null,
		store: gamePageStore,
	};
	setContext(key, ctx);
};
