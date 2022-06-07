import type { HttpClient } from "$utils/http-client";
import type { HubClient } from "$utils/hub-client";
import { getContext, setContext } from "svelte";
import type { AuthService } from "../auth";

const key = Symbol.for("AppContext");

export interface IAppContext {
	http: HttpClient;
	hub: HubClient;
	auth: AuthService;
}

export const getAppContext = () => getContext<IAppContext>(key);

export const initAppContext = (ctx: IAppContext) => {
	setContext(key, ctx);
};
