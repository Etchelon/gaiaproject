import type { Auth0ClientOptions } from "@auth0/auth0-spa-js";

const auth0Config: Auth0ClientOptions = {
	domain: import.meta.env.VITE_AUTH0_DOMAIN!,
	client_id: import.meta.env.VITE_AUTH0_CLIENT_ID!,
	audience: import.meta.env.VITE_AUTH0_AUDIENCE!,
	scope: import.meta.env.VITE_AUTH0_SCOPE!,
	redirect_uri: window.location.origin,
	useRefreshTokens: true,
};

export default auth0Config;
