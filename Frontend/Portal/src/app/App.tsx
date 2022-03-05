import { useAuth0 } from "@auth0/auth0-react";
import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import _ from "lodash";
import { useSnackbar } from "notistack";
import { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import AuthenticatedRoute from "../auth/AuthenticatedRoute";
import { UserInfoDto } from "../dto/interfaces";
import AppFrameRoot from "../frame";
import GamePageRoot from "../game-page";
import UserGamesRoot from "../games";
import { useAppContext } from "../global";
import Home from "../home/Home";
import ManageProfile from "../manage-profile/ManageProfile";
import NewGamePage from "../new-game/NewGamePage";
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
	const { httpClient: http, hubClient: hub } = useAppContext();
	httpClient.setBearerTokenFactory(getAccessTokenSilently);
	hubClient.setBearerTokenFactory(getAccessTokenSilently);
	http.setBearerTokenFactory(getAccessTokenSilently);
	hub.setBearerTokenFactory(getAccessTokenSilently);
	const { enqueueSnackbar } = useSnackbar();
	const [isReady, setIsReady] = useState(false);

	useEffect(() => {
		if (!isAuthenticated) {
			return;
		}

		onAuthenticated(user)
			.then(isFirstLogin => {
				if (isFirstLogin) {
					enqueueSnackbar("It's your first login, you should choose a username and provide other information to compile your profile", {
						variant: "info",
					});
					navigationService.navigateTo("/profile");
				}
				return hubClient.establishConnection();
			})
			.then(() => {
				setIsReady(true);
			});
	}, [isAuthenticated]);

	if (isLoading) {
		return <div></div>;
	}

	return (
		<Router>
			<div className={classes.root}>
				<AppFrameRoot>
					<Routes>
						<Route
							path="/profile"
							element={
								<AuthenticatedRoute>
									<ManageProfile />
								</AuthenticatedRoute>
							}
						/>
						<Route
							path="/games"
							element={
								<AuthenticatedRoute>
									<UserGamesRoot kind="active" />
								</AuthenticatedRoute>
							}
						/>
						<Route
							path="/history"
							element={
								<AuthenticatedRoute>
									<UserGamesRoot kind="finished" />
								</AuthenticatedRoute>
							}
						/>
						<Route
							path="/new-game"
							element={
								<AuthenticatedRoute>
									<NewGamePage />
								</AuthenticatedRoute>
							}
						/>
						<Route path="/game/:id" element={isReady && <GamePageRoot />} />
						<Route path="/" element={<Home />} />
						<Route path="/unauthorized" element={<Unauthorized />} />
						<Route path="*" element={<NotFound />} />
					</Routes>
				</AppFrameRoot>
			</div>
		</Router>
	);
};

export default App;
