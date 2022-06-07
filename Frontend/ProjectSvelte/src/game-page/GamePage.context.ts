import { getContext, setContext } from "svelte";
import type { Writable } from "svelte/store";
import { getAppContext } from "../app/App.context";
import { getGamePageStore, IGamePageStore } from "./GamePage.store";

const key = Symbol.for("GamePageContext");

interface IGamePageContext {
	id: string;
	activeWorkflow: any;
	store: Writable<IGamePageStore>;
}

export const getGamePageContext = () => getContext<IGamePageContext>(key);

export const setGamePageContext = (id: string) => {
	const { http } = getAppContext();
	const store = getGamePageStore(id, http);
	const ctx: IGamePageContext = {
		id,
		activeWorkflow: null,
		store,
	};
	setContext(key, ctx);
};
