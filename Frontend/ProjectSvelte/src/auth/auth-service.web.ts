import type { Auth0ClientOptions, LogoutOptions, RedirectLoginOptions } from "@auth0/auth0-spa-js";
import auth0 from "@auth0/auth0-spa-js";
import { AuthServiceBase, IAuthService, OnRedirectCallback } from "./auth-service.base";
import config from "./config";

const defaultOnRedirectCallback: OnRedirectCallback = (appState: any) => {
	window.history.replaceState({}, document.title, appState && appState.targetUrl ? appState.targetUrl : window.location.pathname);
};

export class AuthServiceWeb extends AuthServiceBase implements IAuthService {
	protected get storage() {
		return window.sessionStorage;
	}

	protected get onRedirectCallback() {
		return defaultOnRedirectCallback;
	}

	initializeAuth0 = (): void => {
		this.createAuth0Client(config).then(() => {
			this.onCallback(window.location.href);
		});
	};

	login = async (options?: RedirectLoginOptions) => {
		await this._auth0Client.loginWithRedirect(options);
	};

	logout = async (options: LogoutOptions) => {
		this._auth0Client.logout({
			...options,
			returnTo: window.location.origin,
		});
	};

	protected onCheckedIfAuthenticated() {
		this.isLoading.set(false);
	}

	private createAuth0Client = async (config: Auth0ClientOptions) => {
		const cacheLocation = "memory";
		this._auth0Client = await auth0({ ...config, cacheLocation });
	};
}
