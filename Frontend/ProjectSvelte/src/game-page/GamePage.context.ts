import { getContext, setContext } from "svelte";
import type { GamePageSignalRConnectionService } from "./GamePageSignalRConnection.service";
import type { GamePageStore } from "./store/GamePage.store";

const key = Symbol.for("GamePageContext");

export interface IGamePageContext {
	id: string;
	store: GamePageStore;
	signalR: GamePageSignalRConnectionService;
	activeWorkflow: any;
}

export const getGamePageContext = () => getContext<IGamePageContext>(key);

export const setGamePageContext = (ctx: IGamePageContext) => {
	setContext(key, ctx);
};
