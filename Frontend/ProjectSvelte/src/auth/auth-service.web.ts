import type { Auth0ClientOptions, LogoutOptions, RedirectLoginOptions } from "@auth0/auth0-spa-js";
import auth0 from "@auth0/auth0-spa-js";
import { AuthServiceBase, IAuthService } from "./auth-service.base";
import config from "./config";

export class AuthServiceWeb extends AuthServiceBase implements IAuthService {
	protected get storage() {
		return window.sessionStorage;
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

	private createAuth0Client = async (config: Auth0ClientOptions) => {
		const cacheLocation = "memory";
		this._auth0Client = await auth0({ ...config, cacheLocation });
	};
}
