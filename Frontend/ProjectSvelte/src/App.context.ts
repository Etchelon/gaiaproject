import { HttpClient } from "$utils/http-client";
import { getContext, setContext } from "svelte";

const key = Symbol.for("AppContext");

interface IAppContext {
	http: HttpClient;
}

export const getAppContext = () => getContext<IAppContext>(key);

export const initAppContext = () => {
	const http = new HttpClient("https://gaiaproject-no-docker.azurewebsites.net");
	const ctx: IAppContext = {
		http,
	};
	setContext(key, ctx);
};
