import { Auth0Provider, Auth0ProviderOptions } from "@auth0/auth0-react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core";
import { SnackbarProvider } from "notistack";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import App from "./app/App";
import "./index.scss";
import store from "./store/store";

const onRedirectCallback = (appState: any) => {
	window.history.pushState({}, "", appState?.returnTo ?? window.location.pathname);
};

const auth0Config: Auth0ProviderOptions = {
	domain: process.env.REACT_APP_AUTH0_DOMAIN!,
	clientId: process.env.REACT_APP_AUTH0_CLIENT_ID!,
	audience: process.env.REACT_APP_AUTH0_AUDIENCE!,
	scope: process.env.REACT_APP_AUTH0_SCOPE!,
	redirectUri: window.location.origin,
	onRedirectCallback,
};

const theme = createMuiTheme({
	palette: {
		type: "dark",
	},
});

ReactDOM.render(
	<Auth0Provider {...auth0Config}>
		<Provider store={store}>
			<SnackbarProvider anchorOrigin={{ horizontal: "center", vertical: "bottom" }}>
				<ThemeProvider theme={theme}>
					<App />
				</ThemeProvider>
			</SnackbarProvider>
		</Provider>
	</Auth0Provider>,
	document.getElementById("gaia-project")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
// reportWebVitals(console.log);
