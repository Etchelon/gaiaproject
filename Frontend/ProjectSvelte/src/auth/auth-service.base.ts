import type { UserInfoDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import type { HubClient } from "$utils/hub-client";
import type { Auth0Client, GetTokenSilentlyOptions, LogoutOptions, RedirectLoginOptions, User } from "@auth0/auth0-spa-js";
import { Browser } from "@capacitor/browser";
import { toastController } from "@ionic/core/components";
import { isNil } from "lodash";
import { push } from "svelte-spa-router";
import { derived, get, Readable, Writable, writable } from "svelte/store";
import { redirectUri } from "./config";

const USER_INFO = "gp-user-info";

export type OnRedirectCallback = (appState?: any) => void;

export interface IAuthService {
	isLoading: Writable<boolean>;
	auth0User: Writable<User | undefined>;
	user: Writable<UserInfoDto | undefined>;
	isAuthenticated: Readable<boolean>;
	loggedUser: Readable<UserInfoDto>;
	error: Writable<Error | null>;
	initializeAuth0(): void;
	login(options?: RedirectLoginOptions): Promise<void>;
	logout(options: LogoutOptions): Promise<void>;
	getAccessToken(options: GetTokenSilentlyOptions): Promise<string>;
}

export abstract class AuthServiceBase implements IAuthService {
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
	protected _auth0Client!: Auth0Client;
	protected abstract get storage(): Storage;

	constructor(protected readonly http: HttpClient, protected readonly hub: HubClient) {}

	getAccessToken = async (options: GetTokenSilentlyOptions) => await this._auth0Client.getTokenSilently(options);

	abstract initializeAuth0(): void;
	abstract login(options?: RedirectLoginOptions): Promise<void>;
	abstract logout(options: LogoutOptions): Promise<void>;

	protected onCallback = async (url: string) => {
		if (!url || !url.startsWith(redirectUri)) {
			return;
		}

		await this.handleAuth0RedirectCallback(url);
		await this.checkIfAuthenticated();
	};

	protected checkIfAuthenticated = async () => {
		try {
			if (await this._auth0Client.isAuthenticated()) {
				await this.handleAuthenticated(this._auth0Client);
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

	private handleAuth0RedirectCallback = async (url: string) => {
		const params = new URL(url).searchParams;
		const hasError = params.has("error");
		const hasCode = params.has("code");
		const hasState = params.has("state");

		if (hasError) {
			this.error.set(new Error(params.get("error_description") || "error completing login process"));
			return;
		}

		if (hasCode && hasState) {
			await this._auth0Client.handleRedirectCallback(url);
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
		const userFromSession = await this.getUserFromStorage();
		if (userFromSession) {
			this.user.set(userFromSession);
			return;
		}

		const { user, isFirstLogin } = await this.isFirstLogin();
		this.saveUserToStorage(user);
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
		this.clearUserFromStorage();
	}

	private getUserFromStorage = () => {
		const userInfoStr = this.storage.getItem(USER_INFO);
		if (!userInfoStr) {
			return null;
		}

		return JSON.parse(userInfoStr) as UserInfoDto;
	};

	private saveUserToStorage = (userInfo: UserInfoDto) => {
		const userInfoStr = JSON.stringify(userInfo);
		this.storage.setItem(USER_INFO, userInfoStr);
	};

	private clearUserFromStorage = () => {
		this.storage.removeItem(USER_INFO);
	};
}
