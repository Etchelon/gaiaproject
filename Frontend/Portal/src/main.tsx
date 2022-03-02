import { Auth0Provider, Auth0ProviderOptions } from "@auth0/auth0-react";
import { createTheme, ThemeProvider } from "@material-ui/core/styles";
import { SnackbarProvider } from "notistack";
import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import App from "./app/App";
import "./index.scss";
import store from "./store/store";

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
		type: "dark",
	},
});

ReactDOM.render(
	<React.StrictMode>
		<Auth0Provider {...auth0Config}>
			<Provider store={store}>
				<SnackbarProvider anchorOrigin={{ horizontal: "center", vertical: "bottom" }}>
					<ThemeProvider theme={theme}>
						<App />
					</ThemeProvider>
				</SnackbarProvider>
			</Provider>
		</Auth0Provider>
	</React.StrictMode>,
	document.getElementById("gaia-project")
);