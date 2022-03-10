import { HubConnection } from "@microsoft/signalr";
import { noop } from "lodash";
import { useSnackbar } from "notistack";
import { useCallback } from "react";
import { GameStateDto } from "../dto/interfaces";
import hubClient from "../utils/hub-client";
import { Nullable } from "../utils/miscellanea";
import { useGamePageContext } from "./GamePage.context";

async function startGameEventsListener(gameId: string): Promise<HubConnection> {
	await hubClient.establishConnection();
	await hubClient.send("JoinGame", gameId);
	return await hubClient.getConnection();
}

async function leaveGame(gameId: string): Promise<Nullable<HubConnection>> {
	try {
		await hubClient.send("LeaveGame", gameId);
		return await hubClient.getConnection();
	} catch (err) {
		console.warn("Hub doesn't see we're leaving the game because of this error", err);
		return null;
	}
}

export const useGamePageSignalRConnection = (gameId: string, closeWorkflowCallback: () => void) => {
	const { vm } = useGamePageContext();
	const { enqueueSnackbar } = useSnackbar();

	const onGameStateChanged = useCallback((newState: string) => {
		closeWorkflowCallback();
		const newGameState = JSON.parse(newState) as GameStateDto;
		vm.gameUpdated(newGameState);
	}, []);

	const onUserJoined = useCallback((userId: string) => {
		vm.userJoined(userId);
	}, []);

	const onUserLeft = useCallback((userId: string) => {
		vm.userLeft(userId);
	}, []);

	const onSetOnlineUsers = useCallback((userIds: string[]) => {
		vm.setOnlineUsers(userIds);
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
		vm.setConnecting();
		startGameEventsListener(gameId)
			.then(connection => {
				connection.onreconnecting = () => {
					vm.setConnecting();
					turnOffListeners(connection);
				};
				connection.onreconnected = () => {
					vm.setConnectedToGame(gameId);
					turnOnListeners(connection);
				};

				vm.setConnectedToGame(gameId);
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
		vm.setDisconnecting();
		leaveGame(gameId).then(connection => {
			if (connection) {
				connection.onreconnecting = noop;
				connection.onreconnected = noop;
				turnOffListeners(connection);
			}
			vm.setDisconnected();
		});
	};

	return { connectToHub, disconnectFromHub };
};
