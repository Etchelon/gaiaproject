<script lang="ts">
	import type { GameInfoDto } from "$dto/interfaces";
	import { trashBin } from "ionicons/icons";
	import { sortBy } from "lodash";
	import { DateTime } from "luxon";
	import GamePlayer from "./GamePlayer.svelte";

	export let game: GameInfoDto;

	const canDelete = true;
	$: creationDate = DateTime.fromISO(game.created).toFormat("MMM d, y");
	$: finishDate = game.ended ? DateTime.fromISO(game.ended).toFormat("MMM d, y") : null;
	$: sortedPlayers = sortBy(game.players, p => p.placement);
</script>

<ion-item-sliding>
	<ion-item href={`#/game/${game.id}`}>
		<ion-label class="gaia-font">
			<h2>{game.name}</h2>
			<p>
				{#if finishDate}
					Finished on {finishDate}
				{:else}
					Started on {creationDate}
				{/if}
			</p>
			<div>
				<div class="game-player-wrapper flex items-center gap-2 md:gap-4 overflow-x-auto overflow-y-hidden">
					{#each sortedPlayers as player (player.id)}
						<div class="flex-shrink-0">
							<GamePlayer {player} />
						</div>
					{/each}
				</div>
			</div>
		</ion-label>
	</ion-item>

	<ion-item-options side="end">
		{#if canDelete}
			<ion-item-option color="danger">
				<ion-icon slot="icon-only" icon={trashBin} />
			</ion-item-option>
		{/if}
	</ion-item-options>
</ion-item-sliding>

<style>
	.game-player-wrapper {
		margin-bottom: -4px;
	}
</style>
