import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import type { BearerTokenFactoryFn } from "./http-client";
import { delay, Nullable } from "./miscellanea";

const isConnecting = (connection_: Nullable<HubConnection>) => {
	if (connection_ === null) {
		return false;
	}
	return connection_.state === HubConnectionState.Connecting || connection_.state === HubConnectionState.Reconnecting;
};
const isConnected = (connection_: Nullable<HubConnection>) => {
	if (connection_ === null) {
		return false;
	}
	return connection_.state === HubConnectionState.Connected;
};

export class HubClient {
	private _hubConnection: Nullable<HubConnection> = null;

	constructor(private readonly baseUrl: string) {}

	private _bearerTokenFactory: BearerTokenFactoryFn = async () => null;

	private async initConnection(): Promise<HubConnection> {
		if (this._hubConnection !== null) {
			return this._hubConnection;
		}
		return (this._hubConnection = new HubConnectionBuilder()
			.withUrl(`${this.baseUrl}/hubs/gaia`, {
				accessTokenFactory: async () => {
					const token = await this._bearerTokenFactory();
					return token ?? "";
				},
			})
			.withAutomaticReconnect()
			.build());
	}

	setBearerTokenFactory(factory: BearerTokenFactoryFn): void {
		this._bearerTokenFactory = factory;
	}

	private async openConnection(): Promise<void> {
		await this._hubConnection?.start();
	}

	async closeConnection(): Promise<void> {
		await this._hubConnection?.stop();
	}

	async establishConnection(): Promise<void> {
		const connection = await this.initConnection();
		if (isConnected(connection)) {
			return;
		}

		if (isConnecting(connection)) {
			let retryCount = 0;
			do {
				await delay(100);
			} while (isConnecting(connection) && retryCount++ < 50);

			if (!isConnected(connection)) {
				throw new Error("Could not connect to the hub within 5000 ms");
			}

			return;
		}

		await this.openConnection();
	}

	async getConnection(): Promise<HubConnection> {
		return await this.initConnection();
	}

	async send(method: string, ...args: any[]): Promise<void> {
		if (!isConnected(this._hubConnection)) {
			return Promise.resolve();
		}
		return await this._hubConnection!.send(method, ...args);
	}
}

const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
const hubClient = new HubClient(actualBaseUrl);
export default hubClient;
