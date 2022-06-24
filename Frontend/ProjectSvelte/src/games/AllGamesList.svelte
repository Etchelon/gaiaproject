<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";
	import type { HttpClient } from "$utils/http-client";

	const listTitles = new Map<GameKind, string>([
		["active", "Ongoing games"],
		["finished", "Finished games"],
	]);

	const fetchAllGames = async (http: HttpClient, kind: GameKind, page: number, pageSize: number) =>
		await http.get<Page<GameInfoDto>>(`api/GaiaProject/AllGames?kind=${kind}&page=${page}&pageSize=${pageSize}`);
</script>

<script lang="ts">
	import LoadingSpinner from "$components/LoadingSpinner.svelte";
	import type { GameInfoDto, Page } from "$dto/interfaces";
	import type { InfiniteScrollCustomEvent } from "@ionic/core";
	import { onMount } from "svelte";
	import { getAppContext } from "../app/App.context";
	import GameListItem from "./GameListItem.svelte";

	export let kind: GameKind;

	const { http } = getAppContext();

	let page = 0;
	let pageSize = 5;
	let hasMore = true;
	let isLoading = false;
	let hasLoaded = false;
	let games: GameInfoDto[] = [];
	$: listTitle = listTitles.get(kind) ?? "";

	const loadData = async (evt?: InfiniteScrollCustomEvent) => {
		isLoading = page === 0;
		try {
			var { items, hasMore: hm } = await fetchAllGames(http, kind, page++, pageSize);
			hasMore = hm;
			games = [...games, ...items];
		} finally {
			evt?.target.complete();
			isLoading = false;
			hasLoaded = true;
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
	{#each games as game (game.id)}
		<GameListItem {game} />
	{:else}
		{#if hasLoaded}
			<ion-item>
				<ion-label class="gaia-font">Nothing here!</ion-label>
			</ion-item>
		{/if}
	{/each}
</ion-list>
<ion-infinite-scroll class="mt-3" disabled={!hasMore} on:ionInfinite={loadData}>
	<ion-infinite-scroll-content loading-spinner="crescent" loading-text="Loading more games..." />
</ion-infinite-scroll>
