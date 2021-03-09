import { HubConnection } from "@microsoft/signalr";
import _ from "lodash";
import { useSnackbar } from "notistack";
import { useCallback } from "react";
import { useDispatch } from "react-redux";
import { GameStateDto } from "../dto/interfaces";
import hubClient from "../utils/hub-client";
import { Nullable } from "../utils/miscellanea";
import { gameUpdated, setConnectedToGame, setConnecting, setDisconnected, setDisconnecting, setOnlineUsers, userJoined, userLeft } from "./store/active-game.slice";

async function startGameEventsListener(gameId: string): Promise<HubConnection> {
	const connection = await hubClient.getConnection();
	if (!connection) {
		throw new Error("Hub Connection unavailable.");
	}

	await connection.send("JoinGame", gameId);
	return connection;
}

async function leaveGame(gameId: string): Promise<Nullable<HubConnection>> {
	if (!hubClient.isConnected()) {
		console.warn("Hub Connection unavailable.");
		return null;
	}

	const connection = await hubClient.getConnection();
	try {
		await connection.send("LeaveGame", gameId);
		return connection;
	} catch (err) {
		console.warn("Hub doesn't see we're leaving the game because of this error", err);
		return null;
	}
}

export const useGamePageSignalRConnection = (gameId: string, closeWorkflowCallback: () => void) => {
	const { enqueueSnackbar } = useSnackbar();
	const dispatch = useDispatch();

	const onGameStateChanged = useCallback((newState: string) => {
		closeWorkflowCallback();
		const newGameState = JSON.parse(newState) as GameStateDto;
		dispatch(gameUpdated(newGameState));
	}, []);

	const onUserJoined = useCallback((userId: string) => {
		dispatch(userJoined(userId));
	}, []);

	const onUserLeft = useCallback((userId: string) => {
		dispatch(userLeft(userId));
	}, []);

	const onSetOnlineUsers = useCallback((userIds: string[]) => {
		dispatch(setOnlineUsers(userIds));
	}, []);

	const turnOffListeners = (connection: HubConnection) => {
		connection.off("GameStateChanged", onGameStateChanged);
		connection.off("UserJoinedGame", onUserJoined);
		connection.off("UserLeftGame", onUserLeft);
		connection.off("SetOnlineUsers", onSetOnlineUsers);
	};
	const turnOnListeners = (connection: HubConnection) => {
		connection.on("GameStateChanged", onGameStateChanged);
		connection.on("UserJoinedGame", onUserJoined);
		connection.on("UserLeftGame", onUserLeft);
		connection.on("SetOnlineUsers", onSetOnlineUsers);
	};

	const connectToHub = () => {
		dispatch(setConnecting());
		startGameEventsListener(gameId)
			.then(connection => {
				connection.onreconnecting = () => {
					dispatch(setConnecting());
					turnOffListeners(connection);
				};
				connection.onreconnected = () => {
					dispatch(setConnectedToGame(gameId));
					turnOnListeners(connection);
				};

				dispatch(setConnectedToGame(gameId));
				turnOffListeners(connection);
				turnOnListeners(connection);
			})
			.catch(err => {
				enqueueSnackbar("Could not connect to the server for updates, try to rejoin the game", {
					variant: "warning",
				});
			});
	};

	const disconnectFromHub = () => {
		dispatch(setDisconnecting());
		leaveGame(gameId).then(connection => {
			if (connection) {
				connection.onreconnecting = _.noop;
				connection.onreconnected = _.noop;
				turnOffListeners(connection);
			}
			dispatch(setDisconnected());
		});
	};

	return { connectToHub, disconnectFromHub };
};
