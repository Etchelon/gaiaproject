<script lang="ts">
	import Page from "$components/Page.svelte";
	import type { GameInfoDto } from "$dto/interfaces";
	import { asyncDelay } from "$utils/miscellanea";
	import { onMount } from "svelte";
	import finishedGamesJson from "./finished-games";

	let finishedGames$: Promise<GameInfoDto[]> = Promise.resolve([]);

	const loadFinishedGames = async () => {
		await asyncDelay(2000);
		return finishedGamesJson;
	};

	onMount(() => {
		finishedGames$ = loadFinishedGames();
	});
</script>

<Page title="Your games">
	{#await finishedGames$}
		<ion-spinner />
	{:then games}
		<ion-list>
			<ion-list-header>Finished games</ion-list-header>
			{#each games as game (game.id)}
				<ion-item href={`#/game/${game.id}`}>
					<ion-label>{game.name}</ion-label>
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
