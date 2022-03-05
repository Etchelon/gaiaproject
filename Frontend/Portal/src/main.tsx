import { Auth0Provider, Auth0ProviderOptions } from "@auth0/auth0-react";
import { createTheme, ThemeProvider, Theme, StyledEngineProvider } from "@mui/material/styles";
import { SnackbarProvider } from "notistack";
import React from "react";
import ReactDOM from "react-dom";
import App from "./app/App";
import { AppProvider } from "./global";
import "./index.scss";

declare module "@mui/styles/defaultTheme" {
	// eslint-disable-next-line @typescript-eslint/no-empty-interface
	interface DefaultTheme extends Theme {}
}

const onRedirectCallback = (appState: any) => {
	window.history.pushState({}, "", appState?.returnTo ?? window.location.pathname);
};

const auth0Config: Auth0ProviderOptions = {
	domain: import.meta.env.VITE_AUTH0_DOMAIN!,
	clientId: import.meta.env.VITE_AUTH0_CLIENT_ID!,
	audience: import.meta.env.VITE_AUTH0_AUDIENCE!,
	scope: import.meta.env.VITE_AUTH0_SCOPE!,
	redirectUri: window.location.origin,
	onRedirectCallback,
};

const theme = createTheme({
	palette: {
		mode: "dark",
	},
});

ReactDOM.render(
	<React.StrictMode>
		<StyledEngineProvider injectFirst>
			<ThemeProvider theme={theme}>
				<SnackbarProvider anchorOrigin={{ horizontal: "center", vertical: "bottom" }}>
					<Auth0Provider {...auth0Config}>
						<AppProvider>
							<App />
						</AppProvider>
					</Auth0Provider>
				</SnackbarProvider>
			</ThemeProvider>
		</StyledEngineProvider>
	</React.StrictMode>,
	document.getElementById("gaia-project")
);
