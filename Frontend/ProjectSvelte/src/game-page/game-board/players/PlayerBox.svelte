<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { isNil } from "lodash";
	import { getGamePageContext } from "../../GamePage.context";
	import { deriveIsOnline } from "../../store/selectors";
	import UninitializedPlayerBox from "./UninitializedPlayerBox.svelte";
	import InitializedPlayerBox from "./InitializedPlayerBox.svelte";

	export let player: PlayerInGameDto;
	export let index: number;

	const { store } = getGamePageContext();
	const isOnline = deriveIsOnline(player.id)(store);

	$: isInitialized = !isNil(player.raceId) && !isNil(player.state);
</script>

{#if !isInitialized}
	<UninitializedPlayerBox {player} {index} isOnline={$isOnline} />
{:else}
	<InitializedPlayerBox {player} isOnline={$isOnline} />
{/if}
