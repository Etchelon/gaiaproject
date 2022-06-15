import type { UserInfoDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import type { HubClient } from "$utils/hub-client";
import type {
	Auth0Client,
	Auth0ClientOptions,
	GetTokenSilentlyOptions,
	LogoutOptions,
	RedirectLoginOptions,
	User,
} from "@auth0/auth0-spa-js";
import auth0 from "@auth0/auth0-spa-js";
import { App } from "@capacitor/app";
import { Browser } from "@capacitor/browser";
import { Capacitor } from "@capacitor/core";
import { toastController } from "@ionic/core/components";
import { isNil } from "lodash";
import { push } from "svelte-spa-router";
import { derived, get, writable } from "svelte/store";
import config, { redirectUri } from "./config";

const USER_INFO = "gp-user-info";

type OnRedirectCallback = (appState?: any) => void;

const defaultOnRedirectCallback: OnRedirectCallback = (appState: any) => {
	window.history.replaceState({}, document.title, appState && appState.targetUrl ? appState.targetUrl : window.location.pathname);
};

export class AuthService {
	isLoading = writable(true);
	auth0User = writable<User | undefined>();
	user = writable<UserInfoDto | undefined>();
	isAuthenticated = derived(this.user, $user => !isNil($user));
	loggedUser = derived([this.isAuthenticated, this.user], ([$isAuthenticated, $user]) => {
		if (!$isAuthenticated || !$user) {
			throw new Error("Not authenticated!");
		}

		return $user as UserInfoDto;
	});
	error = writable<Error | null>(null);
	initializeAuth0: (onRedirectCallback?: OnRedirectCallback) => Promise<void>;
	login: (options?: RedirectLoginOptions) => Promise<void>;
	logout: (options: LogoutOptions) => Promise<void>;
	private _auth0Client!: Auth0Client;

	constructor(private readonly http: HttpClient, private readonly hub: HubClient) {
		const platform = Capacitor.getPlatform();
		console.log(import.meta.env, { redirectUri });
		this.initializeAuth0 = platform === "web" ? this.initializeAuth0Web : this.initializeAuth0App;
		this.login = platform === "web" ? this.loginWeb : this.loginApp;
		this.logout = platform === "web" ? this.logoutWeb : this.logoutApp;
	}

	private initializeAuth0Web = async (onRedirectCallback: OnRedirectCallback = defaultOnRedirectCallback) => {
		if (this._auth0Client) {
			throw new Error("AuthService already initialized!");
		}

		const client = await this.createAuth0Client(config);

		try {
			await this.handleAuth0RedirectCallback(new URLSearchParams(window.location.search), onRedirectCallback);
			if (await client.isAuthenticated()) {
				await this.handleAuthenticated(client);
			} else {
				await this.handleNotAuthenticated();
			}
		} catch (err) {
			this.error.set(err as Error);
			await this.handleNotAuthenticated();
		} finally {
			this.isLoading.set(false);
		}
	};

	private initializeAuth0App = async () => {
		if (this._auth0Client) {
			throw new Error("AuthService already initialized!");
		}

		const client = await this.createAuth0Client(config);

		App.addListener("appUrlOpen", async ({ url }) => {
			if (!url || !url.startsWith(redirectUri)) {
				return;
			}

			try {
				const url_ = new URL(url);
				await this.handleAuth0RedirectCallback(url_.searchParams);
				if (await client.isAuthenticated()) {
					await this.handleAuthenticated(client);
				} else {
					await this.handleNotAuthenticated();
				}
			} catch (err) {
				this.error.set(err as Error);
				await this.handleNotAuthenticated();
			} finally {
				Browser.close();
				this.isLoading.set(false);
			}
		});
		this.isLoading.set(false);
	};

	private loginWeb = async (options?: RedirectLoginOptions) => {
		await this._auth0Client.loginWithRedirect(options);
	};

	private logoutWeb = async (options: LogoutOptions) => {
		this._auth0Client.logout({
			...options,
			returnTo: window.location.origin,
		});
	};

	private loginApp = async (options?: RedirectLoginOptions) => {
		const url = await this._auth0Client.buildAuthorizeUrl(options);
		Browser.open({ url, windowName: "_self" });
	};

	private logoutApp = async (options: LogoutOptions) => {
		const url = await this._auth0Client.buildLogoutUrl(options);
		this._auth0Client.logout({ localOnly: true });
		Browser.open({ url });
	};

	getAccessToken = async (options: GetTokenSilentlyOptions) => await this._auth0Client.getTokenSilently(options);

	private createAuth0Client = async (config: Auth0ClientOptions) => (this._auth0Client = await auth0(config));

	private handleAuth0RedirectCallback = async (params: URLSearchParams, onRedirectCallback?: OnRedirectCallback) => {
		const hasError = params.has("error");
		const hasCode = params.has("code");
		const hasState = params.has("state");

		if (hasError) {
			this.error.set(new Error(params.get("error_description") || "error completing login process"));
			return;
		}

		if (hasCode && hasState) {
			const { appState } = await this._auth0Client.handleRedirectCallback();
			onRedirectCallback?.(appState);
		}
	};

	private isFirstLogin = async () => {
		const auth0User: User = get(this.auth0User)!;
		const result = await this.http.put<{ user: UserInfoDto; isFirstLogin: boolean }>(`api/Users/LoggedIn/${auth0User.sub}`, auth0User);
		if (isNil(result)) {
			throw new Error("Login failed");
		}

		return result;
	};

	private fetchUserInfo = async () => {
		const userFromSession = await this.getFromSessionStorage();
		if (userFromSession) {
			this.user.set(userFromSession);
			return;
		}

		const { user, isFirstLogin } = await this.isFirstLogin();
		this.saveToSessionStorage(user);
		this.user.set(user);
		if (!isFirstLogin) {
			return;
		}

		const snack = await toastController.create({
			message: "It's your first login, you should choose a username and provide other information to compile your profile",
		});
		await snack.present();
		push("/profile");
	};

	private getFromSessionStorage = () => {
		const userInfoStr = window.sessionStorage.getItem(USER_INFO);
		if (!userInfoStr) {
			return null;
		}

		return JSON.parse(userInfoStr) as UserInfoDto;
	};

	private saveToSessionStorage = (userInfo: UserInfoDto) => {
		const userInfoStr = JSON.stringify(userInfo);
		window.sessionStorage.setItem(USER_INFO, userInfoStr);
	};

	private clearFromSessionStorage = () => {
		window.sessionStorage.removeItem(USER_INFO);
	};

	private async handleAuthenticated(client: Auth0Client) {
		this.http.setBearerTokenFactory(() => this.getAccessToken({}));
		this.hub.setBearerTokenFactory(() => this.getAccessToken({}));
		this.auth0User.set(await client.getUser());
		await this.fetchUserInfo();
	}

	private async handleNotAuthenticated() {
		this.http.setBearerTokenFactory(async () => null);
		this.hub.setBearerTokenFactory(async () => null);
		this.auth0User.set(void 0);
		this.user.set(void 0);
		this.clearFromSessionStorage();
	}
}
