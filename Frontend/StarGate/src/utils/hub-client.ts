import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import _ from "lodash";
import { timer } from "rxjs";
import { BASE_URL } from "../config";
import { BearerTokenFactoryFn } from "./http-client";
import { Nullable } from "./miscellanea";

const isConnecting = (connection_: HubConnection) => connection_.state === HubConnectionState.Connecting || connection_.state === HubConnectionState.Reconnecting;
const isConnected = (connection_: HubConnection) => connection_.state === HubConnectionState.Connected;

class HubClient {
	private _hubConnection: Nullable<HubConnection> = null;

	constructor(private readonly baseUrl: string) {}

	private _bearerTokenFactory: BearerTokenFactoryFn = async () => null;

	private async initConnection(): Promise<void> {
		if (!_.isNull(this._hubConnection)) {
			return;
		}
		this._hubConnection = new HubConnectionBuilder()
			.withUrl(`${this.baseUrl}/hubs/gaia`, {
				accessTokenFactory: async () => {
					const token = await this._bearerTokenFactory();
					return token ?? "";
				},
			})
			.withAutomaticReconnect()
			.build();
	}

	setBearerTokenFactory(factory: BearerTokenFactoryFn): void {
		this._bearerTokenFactory = factory;
	}

	async openConnection(): Promise<void> {
		await this.initConnection();
		await this._hubConnection!.start();
	}

	async closeConnection(): Promise<void> {
		await this._hubConnection?.stop();
	}

	async ensureConnected(): Promise<void> {
		await this.initConnection();
		const connection = this._hubConnection!;
		if (isConnected(connection)) {
			return;
		}

		if (isConnecting(connection)) {
			const getSleeper = () => timer(100);
			let retryCount = 0;
			do {
				await getSleeper().toPromise();
			} while (isConnecting(connection) && retryCount++ < 50);

			if (!isConnected(connection)) {
				throw new Error("Could not connect to the hub within 5000 ms");
			}

			return;
		}

		await connection.start();
	}

	isConnected(): boolean {
		return this._hubConnection?.state === HubConnectionState.Connected;
	}

	async getConnection(): Promise<HubConnection> {
		if (this._hubConnection?.state !== HubConnectionState.Connected) {
			await this.initConnection();
		}
		return this._hubConnection!;
	}
}

const actualBaseUrl = BASE_URL.replace("<hostname>", window.location.hostname);
const hubClient = new HubClient(actualBaseUrl);
export default hubClient;
