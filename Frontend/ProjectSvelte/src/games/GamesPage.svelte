<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";
	import type { HttpClient } from "$utils/http-client";
	import { orderBy } from "lodash";

	const gameKindLabels = new Map<GameKind, string>([
		["active", "Active Games"],
		["waiting", "Waiting for You"],
		["finished", "Finished Games"],
	]);

	const fetchGames = async (http: HttpClient, kind: GameKind) => {
		const games = await http.get<GameInfoDto[]>(`api/GaiaProject/GetUserGames?kind=${kind}`);
		return orderBy(games, [g => g.created], ["desc"]);
	};
</script>

<script lang="ts">
	import Page from "$components/Page.svelte";
	import type { GameInfoDto } from "$dto/interfaces";
	import { getAppContext } from "../App.context";

	export let kind: GameKind;

	const { http } = getAppContext();

	let games$: Promise<GameInfoDto[]> = Promise.resolve([]);

	$: {
		games$ = fetchGames(http, kind);
	}
</script>

<Page title="Your games">
	{#await games$}
		<div class="w-full h-10 p-4 flex justify-center">
			<ion-spinner />
		</div>
	{:then games}
		<ion-list>
			<ion-list-header>{gameKindLabels.get(kind)}</ion-list-header>
			{#each games as game (game.id)}
				<ion-item href={`#/game/${game.id}`}>
					<ion-label>{game.name}</ion-label>
				</ion-item>
			{:else}
				<ion-item>
					<ion-label>Nothing here!</ion-label>
				</ion-item>
			{/each}
		</ion-list>
	{:catch error}
		<div class="text-white gaia-font">
			<p>Could not fetch finished games</p>
			<small>{error?.message ?? error}</small>
		</div>
	{/await}
</Page>
