import type { UserInfoDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import type {
	Auth0Client,
	Auth0ClientOptions,
	GetTokenSilentlyOptions,
	LogoutOptions,
	RedirectLoginOptions,
	User,
} from "@auth0/auth0-spa-js";
import auth0 from "@auth0/auth0-spa-js";
import { toastController } from "@ionic/core/components";
import { isNil } from "lodash";
import { push } from "svelte-spa-router";
import { derived, get, writable } from "svelte/store";
import config from "./config";

type OnRedirectCallback = (appState?: any) => void;

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
	private _auth0Client!: Auth0Client;

	constructor(private readonly http: HttpClient) {}

	initializeAuth0 = async (onRedirectCallback?: OnRedirectCallback) => {
		const client = await this.createAuth0Client(config);

		const cb = onRedirectCallback ?? (() => window.history.replaceState({}, document.title, window.location.pathname));
		try {
			await this.handleAuth0RedirectCallback(cb);
			this.http.setBearerTokenFactory(() => this.getAccessToken({}));
			this.auth0User.set(await client.getUser());
			await this.checkFirstLogin();
		} catch (err) {
			this.error.set(err as Error);
			this.http.setBearerTokenFactory(async () => null);
			this.auth0User.set(void 0);
		} finally {
			this.isLoading.set(false);
		}
	};

	login = async (options?: RedirectLoginOptions) => {
		await this._auth0Client?.loginWithRedirect(options);
	};

	logout = async (options: LogoutOptions) => {
		this._auth0Client?.logout(options);
	};

	getAccessToken = async (options: GetTokenSilentlyOptions) => await this._auth0Client.getTokenSilently(options);

	private createAuth0Client = async (config: Auth0ClientOptions) => (this._auth0Client = await auth0(config));

	private handleAuth0RedirectCallback = async (onRedirectCallback: OnRedirectCallback) => {
		const params = new URLSearchParams(window.location.search);
		const hasError = params.has("error");
		const hasCode = params.has("code");
		const hasState = params.has("state");

		if (hasError) {
			this.error.set(new Error(params.get("error_description") || "error completing login process"));
			return;
		}

		if (hasCode && hasState) {
			const { appState } = await this._auth0Client.handleRedirectCallback();
			onRedirectCallback(appState);
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

	private checkFirstLogin = async () => {
		const { user, isFirstLogin } = await this.isFirstLogin();
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
}
