import { useAuth0 } from "@auth0/auth0-react";
import { createMuiTheme, createStyles, makeStyles, ThemeProvider } from "@material-ui/core";
import _ from "lodash";
import { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import AuthenticatedRoute from "../auth/AuthenticatedRoute";
import { UserInfoDto } from "../dto/interfaces";
import AppFrame from "../frame/AppFrame";
import GamePage from "../game-page/GamePage";
import UserGames from "../games/UserGames";
import Home from "../home/Home";
import ManageProfile from "../manage-profile/ManageProfile";
import NewGamePage from "../new-game/NewGamePage";
import ToastManager, { Toast, ToastContext } from "../toast/ToastManager";
import Unauthorized from "../unauthorized/Unauthorized";
import httpClient from "../utils/http-client";
import hubClient from "../utils/hub-client";
import navigationService from "../utils/navigation.service";
import NotFound from "../utils/NotFound";
import userInfoService from "../utils/user-info.service";

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			display: "flex",
			flexDirection: "column",
			width: "100vw",
			height: "100vh",
			overflowX: "hidden",
			overflowY: "auto",
		},
	})
);

const theme = createMuiTheme({
	palette: {
		type: "dark",
	},
});

const onAuthenticated = async (auth0User: any): Promise<boolean> => {
	const result = await httpClient.put<{ user: UserInfoDto; isFirstLogin: boolean }>(`api/Users/LoggedIn/${auth0User.sub}`, auth0User);
	if (_.isNil(result)) {
		throw new Error("Login failed");
	}
	userInfoService.store(auth0User.sub, result!.user);
	return result.isFirstLogin;
};

const App = () => {
	const classes = useStyles();
	const { getAccessTokenSilently, isAuthenticated, isLoading, user } = useAuth0();
	httpClient.setBearerTokenFactory(getAccessTokenSilently);
	hubClient.setBearerTokenFactory(getAccessTokenSilently);

	const [toasts, setToasts] = useState<Toast[]>([]);
	const openToast = (t: Toast) => {
		t.id = _.uniqueId("app_toast_");
		setToasts([...toasts, t]);
	};
	const closeToast = (id: string) => {
		const toasts_ = _.reject(toasts, t => t.id === id);
		setToasts(toasts_);
	};

	const [isReady, setIsReady] = useState(false);

	useEffect(() => {
		if (!isAuthenticated) {
			return;
		}

		Promise.all([hubClient.openConnection(), onAuthenticated(user)]).then(results => {
			const [__, isFirstLogin] = results;
			if (isFirstLogin) {
				openToast({ type: "info", message: "It's your first login, you should choose a username and provide other information to compile your profile" });
				navigationService.navigateTo("/profile");
				return;
			}

			setIsReady(true);
		});
	}, [isAuthenticated]);

	if (isLoading) {
		return <div></div>;
	}

	return (
		<ThemeProvider theme={theme}>
			<ToastContext.Provider value={{ toasts, open: openToast, close: closeToast }}>
				<Router>
					<div className={classes.root}>
						<AppFrame>
							<Switch>
								<AuthenticatedRoute path="/profile">
									<ManageProfile />
								</AuthenticatedRoute>
								<AuthenticatedRoute path="/games">
									<UserGames kind="active" />
								</AuthenticatedRoute>
								<AuthenticatedRoute path="/history">
									<UserGames kind="finished" />
								</AuthenticatedRoute>
								<AuthenticatedRoute path="/new-game">
									<NewGamePage />
								</AuthenticatedRoute>
								<Route path="/game/:id">{isReady && <GamePage />}</Route>
								<Route exact path="/">
									<Home />
								</Route>
								<Route exact path="/unauthorized">
									<Unauthorized />
								</Route>
								<Route path="*">
									<NotFound />
								</Route>
							</Switch>
						</AppFrame>
					</div>
				</Router>
				<ToastManager />
			</ToastContext.Provider>
		</ThemeProvider>
	);
};

export default App;
