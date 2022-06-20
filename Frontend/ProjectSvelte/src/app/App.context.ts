import type { HttpClient } from "$utils/http-client";
import type { HubClient } from "$utils/hub-client";
import { getContext, setContext } from "svelte";
import type { IAuthService } from "../auth/auth-service.base";

const key = Symbol.for("AppContext");

export type PlatformType = "web" | "android" | "ios";

export interface IAppContext {
	platform: PlatformType;
	http: HttpClient;
	hub: HubClient;
	auth: IAuthService;
}

export const getAppContext = () => getContext<IAppContext>(key);

export const initAppContext = (ctx: IAppContext) => {
	setContext(key, ctx);
};
