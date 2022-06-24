<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";
	import type { HttpClient } from "$utils/http-client";

	const listTitles = new Map<GameKind, string>([
		["active", "Ongoing games"],
		["finished", "Finished games"],
	]);

	const fetchAllGames = async (http: HttpClient, kind: GameKind, skip: number, take: number) =>
		await http.get<Page<GameInfoDto>>(`api/GaiaProject/AllGames?kind=${kind}&skip=${skip}&take=${take}`);
</script>

<script lang="ts">
	import LoadingSpinner from "$components/LoadingSpinner.svelte";
	import type { GameInfoDto, Page } from "$dto/interfaces";
	import { getAppContext } from "../app/App.context";
	import GameListItem from "./GameListItem.svelte";
	import { onMount } from "svelte";
	import { writable } from "svelte/store";

	export let kind: GameKind;

	const { http } = getAppContext();

	let skip = 0;
	let take = 5;
	let hasMore = true;
	let isLoading = false;
	const games = writable<GameInfoDto[]>([]);
	$: listTitle = listTitles.get(kind) ?? "";

	const loadData = async () => {
		isLoading = skip === 0;
		try {
			var { items, hasMore: hm } = await fetchAllGames(http, kind, skip++, take);
			hasMore = hm;
			games.update(current => [...current, ...items]);
		} finally {
			isLoading = false;
		}
	};

	onMount(() => {
		loadData();
	});
</script>

{#if isLoading}
	<LoadingSpinner />
{/if}

<ion-list>
	<ion-list-header class="gaia-font">{listTitle}</ion-list-header>
	{#each $games as game (game.id)}
		<GameListItem {game} />
	{:else}
		<ion-item>
			<ion-label class="gaia-font">Nothing here!</ion-label>
		</ion-item>
	{/each}
</ion-list>
<ion-infinite-scroll disabled={!hasMore} on:ionInfinite={loadData}>
	<ion-infinite-scroll-content loading-spinner="crescent" loading-text="Loading more games..." />
</ion-infinite-scroll>
