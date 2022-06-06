<script lang="ts">
	import type { GameInfoDto } from "$dto/interfaces";
	import { trashBin } from "ionicons/icons";
	import { chain } from "lodash";
	import { DateTime } from "luxon";
	import GamePlayer from "./GamePlayer.svelte";

	export let game: GameInfoDto;

	const canDelete = true;
	$: creationDate = DateTime.fromISO(game.created).toFormat("MMM d, y");
	$: finishDate = game.ended ? DateTime.fromISO(game.ended).toFormat("MMM d, y") : null;
	$: winnersNames = finishDate
		? chain(game.players)
				.filter(p => p.placement === 1)
				.map(p => p.username)
				.value()
				.join(", ")
		: null;
</script>

<ion-item-sliding>
	<ion-item href={`#/game/${game.id}`}>
		<ion-label>
			<h3>{game.name}</h3>
			<p>
				{#if finishDate}
					Finished on {finishDate}, {winnersNames} won!
				{:else}
					Started on {creationDate} by {game.createdBy.username}
				{/if}
			</p>
			<div class="flex items-center gap-2 md:gap-4">
				{#each game.players as player (player.id)}
					<GamePlayer {player} />
				{/each}
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
