import { orderBy } from "lodash";
import { makeAutoObservable } from "mobx";
import { GameInfoDto } from "../../dto/interfaces";
import { HttpClient } from "../../utils/http-client";
import { Nullable } from "../../utils/miscellanea";
import { GameKind, LoadingStatus } from "./types";

export class GamesViewModel {
	status: LoadingStatus = "idle";
	get isLoading() {
		return this.status === "loading";
	}

	games: GameInfoDto[] = [];
	get gamesIds() {
		return this.games.map(g => g.id);
	}

	lastFetchParams: Nullable<{ kind: GameKind }> = null;
	lastFetched: Nullable<string> = null;
	error: Nullable<string> = null;
	deleteGameProgress: LoadingStatus = "idle";

	constructor(private readonly httpClient: HttpClient) {
		makeAutoObservable(this);
	}

	async fetchGames(kind: GameKind) {
		this.status = "loading";
		try {
			const games = await this.httpClient.get<GameInfoDto[]>(`api/GaiaProject/GetUserGames?kind=${kind}`);
			this.lastFetched = new Date().toISOString();
			this.lastFetchParams = { kind };
			this.games = orderBy(games, [g => g.created], ["desc"]);
			this.status = "success";
		} catch (err) {
			this.status = "failure";
		}
	}

	async deleteGame(id: string) {
		this.deleteGameProgress = "loading";
		try {
			await this.httpClient.delete(`api/GaiaProject/DeleteGame/${id}`);
			this.deleteGameProgress = "success";
		} catch (err) {
			this.deleteGameProgress = "failure";
		}
	}
}
