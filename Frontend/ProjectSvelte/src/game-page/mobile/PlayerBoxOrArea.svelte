<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import PlayerArea from "../game-board/players/PlayerArea.svelte";
	import PlayerBox from "../game-board/players/PlayerBox.svelte";

	export let player: PlayerInGameDto;
	export let index: number;
	export let forcePlayerAreaView = false;

	let showArea = false;

	$: {
		if (forcePlayerAreaView) {
			showArea = true;
		}
	}
</script>

<div on:click|stopPropagation={() => (showArea = !showArea)}>
	{#if showArea}
		<PlayerArea {player} framed={true} />
	{:else}
		<PlayerBox {player} {index} />
	{/if}
</div>
