import type { Auth0ClientOptions, LogoutOptions, RedirectLoginOptions } from "@auth0/auth0-spa-js";
import auth0 from "@auth0/auth0-spa-js";
import { App } from "@capacitor/app";
import { Browser } from "@capacitor/browser";
import { AuthServiceBase, IAuthService } from "./auth-service.base";
import config, { redirectUri } from "./config";

export class AuthServiceApp extends AuthServiceBase implements IAuthService {
	protected get storage() {
		return window.localStorage;
	}

	protected get onRedirectCallback() {
		return void 0;
	}

	initializeAuth0 = (): void => {
		App.addListener("appUrlOpen", ({ url }) => this.onCallback(url));
		this.createAuth0Client(config)
			.then(() => {
				this.checkIfAuthenticated();
			})
			.finally(() => Browser.close());
	};

	login = async (options?: RedirectLoginOptions) => {
		const url = await this._auth0Client.buildAuthorizeUrl(options);
		await Browser.open({ url, windowName: "_self" });
	};

	logout = async (options: LogoutOptions) => {
		const url = this._auth0Client.buildLogoutUrl({ ...options, returnTo: redirectUri });
		console.log(url);
		await this._auth0Client.logout({ localOnly: true });
		await Browser.open({ url });
	};

	protected async onCheckedIfAuthenticated() {
		this.isLoading.set(false);
		await Browser.close();
	}

	private createAuth0Client = async (config: Auth0ClientOptions) => {
		const cacheLocation = "localstorage";
		this._auth0Client = await auth0({ ...config, cacheLocation });
	};
}
