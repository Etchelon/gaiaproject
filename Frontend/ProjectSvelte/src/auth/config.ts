import type { Auth0ClientOptions } from "@auth0/auth0-spa-js";
import { Capacitor } from "@capacitor/core";
import config from "../../capacitor.config";

const domain = import.meta.env.VITE_AUTH0_DOMAIN;
export const redirectUri =
	Capacitor.getPlatform() === "web" ? window.location.origin : `${config.appId}://${domain}/capacitor/${config.appId}/callback`;

const auth0Config: Auth0ClientOptions = {
	domain,
	client_id: import.meta.env.VITE_AUTH0_CLIENT_ID,
	audience: import.meta.env.VITE_AUTH0_AUDIENCE,
	scope: import.meta.env.VITE_AUTH0_SCOPE,
	redirect_uri: redirectUri,
	useRefreshTokens: true,
};

export default auth0Config;
