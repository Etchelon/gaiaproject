import { getContext, setContext } from "svelte";
import type { GamePageSignalRConnectionService } from "./GamePageSignalRConnection.service";
import type { GamePageService } from "./GamePage.service";

const key = Symbol.for("GamePageContext");

export interface IGamePageContext {
	id: string;
	store: GamePageService;
	signalR: GamePageSignalRConnectionService;
	activeWorkflow: any;
}

export const getGamePageContext = () => getContext<IGamePageContext>(key);

export const setGamePageContext = (ctx: IGamePageContext) => {
	setContext(key, ctx);
};
