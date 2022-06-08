import { getContext, setContext } from "svelte";
import type { Writable } from "svelte/store";
import type { GamePageSignalRConnectionService } from "./GamePageSignalRConnection.service";
import type { GamePageStore } from "./store/GamePage.store";
import type { ActionWorkflow } from "./workflows/action-workflow.base";

const key = Symbol.for("GamePageContext");

export interface IGamePageContext {
	id: string;
	store: GamePageStore;
	signalR: GamePageSignalRConnectionService;
	activeWorkflow: Writable<ActionWorkflow | null>;
	startWorkflow: (wf: ActionWorkflow) => void;
	closeWorkflow: () => void;
}

export const getGamePageContext = () => getContext<IGamePageContext>(key);

export const setGamePageContext = (ctx: IGamePageContext) => {
	setContext(key, ctx);
};
