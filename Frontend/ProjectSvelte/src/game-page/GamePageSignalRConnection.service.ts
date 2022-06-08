import type { HubClient } from "$utils/hub-client";
import type { HubConnection } from "@microsoft/signalr";
import { noop } from "lodash";
import { get } from "svelte/store";
import type { GameStateDto } from "../dto/interfaces";
import type { Nullable } from "../utils/miscellanea";
import type { GamePageService } from "./GamePage.service";

export class GamePageSignalRConnectionService {
	constructor(
		private readonly hub: HubClient,
		private readonly store: GamePageService,
		private readonly closeWorkflowCallback: () => void
	) {}

	connectToHub = async () => {
		const game = get(this.store.game);
		if (!game) {
			throw new Error("WTF");
		}

		this.store.setConnecting();
		try {
			const connection = await this.startGameEventsListener(game.id);
			connection.onreconnecting = () => {
				this.store.setConnecting();
				this.turnOffListeners(connection);
			};
			connection.onreconnected = () => {
				this.store.setConnectedToGame(game.id);
				this.turnOnListeners(connection);
			};

			this.store.setConnectedToGame(game.id);
			this.turnOffListeners(connection);
			this.turnOnListeners(connection);
		} catch (err) {
			this.enqueueSnackbar("Could not connect to the server for updates, try to rejoin the game", {
				variant: "warning",
			});
		}
	};

	disconnectFromHub = async () => {
		const game = get(this.store.game);
		if (!game) {
			throw new Error("WTF");
		}

		this.store.setDisconnecting();
		const connection = await this.leaveGame(game.id);
		if (connection) {
			connection.onreconnecting = noop;
			connection.onreconnected = noop;
			this.turnOffListeners(connection);
		}
		this.store.setDisconnected();
	};

	private onGameStateChanged = (newState: string) => {
		this.closeWorkflowCallback();
		const newGameState = JSON.parse(newState) as GameStateDto;
		this.store.updateGameState(newGameState);
	};

	private onUserJoined = (userId: string) => {
		this.store.userJoined(userId);
	};

	private onUserLeft = (userId: string) => {
		this.store.userLeft(userId);
	};

	private onSetOnlineUsers = (userIds: string[]) => {
		this.store.setOnlineUsers(userIds);
	};

	private turnOffListeners = (connection: HubConnection) => {
		connection.off("GameStateChanged", this.onGameStateChanged);
		connection.off("UserJoinedGame", this.onUserJoined);
		connection.off("UserLeftGame", this.onUserLeft);
		connection.off("SetOnlineUsers", this.onSetOnlineUsers);
	};
	private turnOnListeners = (connection: HubConnection) => {
		connection.on("GameStateChanged", this.onGameStateChanged);
		connection.on("UserJoinedGame", this.onUserJoined);
		connection.on("UserLeftGame", this.onUserLeft);
		connection.on("SetOnlineUsers", this.onSetOnlineUsers);
	};

	private startGameEventsListener = async (gameId: string): Promise<HubConnection> => {
		await this.hub.establishConnection();
		await this.hub.send("JoinGame", gameId);
		return await this.hub.getConnection();
	};

	private leaveGame = async (gameId: string): Promise<Nullable<HubConnection>> => {
		try {
			await this.hub.send("LeaveGame", gameId);
			return await this.hub.getConnection();
		} catch (err) {
			console.warn("Hub doesn't see we're leaving the game because of this error", err);
			return null;
		}
	};

	private enqueueSnackbar(arg0: string, arg1: { variant: string }) {
		throw new Error("Method not implemented.");
	}
}
