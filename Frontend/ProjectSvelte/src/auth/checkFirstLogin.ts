import type { UserInfoDto } from "$dto/interfaces";
import type { HttpClient } from "$utils/http-client";
import type { User } from "@auth0/auth0-spa-js";
import { toastController } from "@ionic/core/components";
import { isNil } from "lodash";
import { get } from "svelte/store";
import { push } from "svelte-spa-router";
import { getAppContext } from "../App.context";
import { useAuth0 } from ".";

const isFirstLogin = async (http: HttpClient, auth0User: User) => {
	const result = await http.put<{ user: UserInfoDto; isFirstLogin: boolean }>(`api/Users/LoggedIn/${auth0User.sub}`, auth0User);
	if (isNil(result)) {
		throw new Error("Login failed");
	}

	return result;
};

const checkFirstLogin = async () => {
	const { http } = getAppContext();
	const { isAuthenticated, user } = useAuth0;
	if (!get(isAuthenticated)) {
		http.setBearerTokenFactory(async () => null);
		return;
	}

	http.setBearerTokenFactory(() => useAuth0.getAccessToken({}));
	const { user: userDto, isFirstLogin: isFirst } = await isFirstLogin(http, get(user)!);
	if (!isFirst) {
		return;
	}

	const snack = await toastController.create({
		message: "It's your first login, you should choose a username and provide other information to compile your profile",
	});
	await snack.present();
	push("/profile");
};

export default checkFirstLogin;
