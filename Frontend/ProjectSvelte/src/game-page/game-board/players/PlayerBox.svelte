<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { isNil, random } from "lodash";
	import UninitializedPlayerBox from "./UninitializedPlayerBox.svelte";
	import InitializedPlayerBox from "./InitializedPlayerBox.svelte";

	export let player: PlayerInGameDto;
	export let index: number;
	$: isInitialized = !isNil(player.raceId) && !isNil(player.state);
	let isOnline = random(true) > 0.5;
</script>

{#if !isInitialized}
	<UninitializedPlayerBox {player} {index} {isOnline} />
{:else}
	<InitializedPlayerBox {player} {index} {isOnline} />
{/if}
