<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";
	import type { HttpClient } from "$utils/http-client";
	import { orderBy } from "lodash";

	const listTitles = new Map<GameKind, string>([
		["active", "Active games"],
		["waiting", "Waiting for you"],
		["finished", "Finished games"],
	]);

	const fetchGames = async (http: HttpClient, kind: GameKind) => {
		const games = await http.get<GameInfoDto[]>(`api/GaiaProject/UserGames?kind=${kind}`);
		return orderBy(games, [g => g.created], ["desc"]);
	};
</script>

<script lang="ts">
	import LoadingSpinner from "$components/LoadingSpinner.svelte";
	import type { GameInfoDto } from "$dto/interfaces";
	import { getAppContext } from "../app/App.context";
	import GameListItem from "./GameListItem.svelte";

	export let kind: GameKind;

	const { http } = getAppContext();

	let games$: Promise<GameInfoDto[]> = Promise.resolve([]);
	$: {
		games$ = fetchGames(http, kind);
	}
	$: listTitle = listTitles.get(kind) ?? "";
</script>

{#await games$}
	<LoadingSpinner />
{:then games}
	<ion-list>
		<ion-list-header class="gaia-font">{listTitle}</ion-list-header>
		{#each games as game (game.id)}
			<GameListItem {game} />
		{:else}
			<ion-item>
				<ion-label>Nothing here!</ion-label>
			</ion-item>
		{/each}
	</ion-list>
{:catch error}
	<div class="text-white gaia-font">
		<p>Could not fetch games</p>
		<small>{error?.message ?? error}</small>
	</div>
{/await}
