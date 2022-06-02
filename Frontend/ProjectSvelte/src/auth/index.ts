import auth0 from "@auth0/auth0-spa-js";
import type {
	Auth0Client,
	Auth0ClientOptions,
	GetTokenSilentlyOptions,
	LogoutOptions,
	RedirectLoginOptions,
	User,
} from "@auth0/auth0-spa-js";
import { writable } from "svelte/store";
import config from "./config";

type OnRedirectCallback = (appState?: any) => void;

const _useAuth0 = () => {
	let auth0Client: Auth0Client;
	const isAuthenticated = writable(false);
	const isLoading = writable(true);
	const user = writable<User | undefined>();
	const error = writable<Error | null>(null);

	const createAuth0Client = async (config: Auth0ClientOptions) => {
		auth0Client = await auth0(config);
	};

	const handleAuth0RedirectCallback = async (onRedirectCallback: OnRedirectCallback) => {
		const params = new URLSearchParams(window.location.search);
		const hasError = params.has("error");
		const hasCode = params.has("code");
		const hasState = params.has("state");

		if (hasError) {
			error.set(new Error(params.get("error_description") || "error completing login process"));

			return;
		}

		if (hasCode && hasState) {
			const { appState } = await auth0Client.handleRedirectCallback();
			onRedirectCallback(appState);
		}
	};

	const initializeAuth0 = async (onRedirectCallback?: OnRedirectCallback) => {
		await createAuth0Client(config);

		const cb = onRedirectCallback ?? (() => window.history.replaceState({}, document.title, window.location.pathname));
		try {
			await handleAuth0RedirectCallback(cb);
		} catch (err) {
			error.set(err as Error);
		} finally {
			isAuthenticated.set(await auth0Client.isAuthenticated());
			user.set(await auth0Client.getUser());
			isLoading.set(false);
		}
	};

	const login = async (options?: RedirectLoginOptions) => {
		if (!auth0Client) {
			return;
		}

		await auth0Client.loginWithRedirect(options);
	};

	const logout = async (options: LogoutOptions) => {
		auth0Client.logout(options);
	};

	const getAccessToken = async (options: GetTokenSilentlyOptions) => {
		return await auth0Client.getTokenSilently(options);
	};

	return {
		isAuthenticated,
		isLoading,
		user,

		createAuth0Client,
		initializeAuth0,
		login,
		logout,
		getAccessToken,
	};
};

export const useAuth0 = _useAuth0();
