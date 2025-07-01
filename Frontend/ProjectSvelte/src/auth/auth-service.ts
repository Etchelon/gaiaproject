import type { UserInfoDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import type { HubClient } from "$utils/hub-client";
import { toastController } from "@ionic/core/components";
import type { User } from "@supabase/supabase-js";
import { isNil } from "lodash";
import { push } from "svelte-spa-router";
import { derived, get, Readable, Writable, writable } from "svelte/store";
import type { Database } from "../supabase/db";
import type { TypedSupabaseClient } from "../supabase/types";

export type OnRedirectCallback = (appState?: any) => void;

export interface IAuthService {
	isLoading: Writable<boolean>;
	user: Writable<UserInfoDto | undefined>;
	isAuthenticated: Readable<boolean>;
	loggedUser: Readable<UserInfoDto>;
	error: Writable<Error | null>;
	login(email: string): Promise<void>;
	logout(): Promise<void>;
	updateUser(user: UserInfoDto): void;
}

export class AuthService implements IAuthService {
	isLoading = writable(true);
	private supabaseUser = writable<User | undefined>();
	user = writable<UserInfoDto | undefined>();
	private _refreshToken: string | undefined;
	isAuthenticated = derived(this.supabaseUser, $user => !isNil($user));
	loggedUser = derived([this.isAuthenticated, this.user], ([$isAuthenticated, $user]) => {
		if (!$isAuthenticated || !$user) {
			throw new Error("Not authenticated!");
		}

		return $user as UserInfoDto;
	});
	error = writable<Error | null>(null);

	constructor(private readonly supabase: TypedSupabaseClient, private readonly http: HttpClient, private readonly hub: HubClient) {}

	updateUser(user: UserInfoDto): void {
		this.user.set(user);
	}

	initializeSupabase = () => {
		this.supabase.auth.onAuthStateChange(async (event, session) => {
			if (event === "SIGNED_IN" && session) {
				this._refreshToken = session.refresh_token;
				this.http.setBearerTokenFactory(async () => session.access_token);
				this.hub.setBearerTokenFactory(async () => session.access_token);
				this.supabaseUser.set(session.user);
				await this.fetchUserInfo();
			} else if (event === "SIGNED_OUT") {
				this.http.setBearerTokenFactory(async () => null);
				this.hub.setBearerTokenFactory(async () => null);
				this.supabaseUser.set(undefined);
				this.user.set(void 0);
			}
		});
	};

	login = async (email: string) => {
		await this.supabase.auth.signInWithOtp({ email: email ?? "andrea.bertoldo.btf@outlook.com" });
	};

	logout = async () => {
		await this.supabase.auth.signOut();
	};

	private isFirstLogin = async () => {
		const su = get(this.supabaseUser);
		if (!su) {
			return { user: void 0, isFirstLogin: false };
		}

		const { data: profile } = await this.supabase.from("profiles").select("*").eq("id", su.id).single();
		return { user: profile ? fromDbProfile(profile) : void 0, isFirstLogin: profile != null };
	};

	private fetchUserInfo = async () => {
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

function fromDbProfile(profile: Database["public"]["Tables"]["profiles"]["Row"]): UserInfoDto {
	return {
		id: profile.id,
		avatar: profile.avatar_url ?? "/public/assets/person.png",
		username: profile.username,
		firstName: profile.first_name ?? "",
		lastName: profile.last_name ?? "",
		memberSince: new Date().toISOString(),
	};
}
