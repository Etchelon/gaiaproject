<script lang="ts">
	import type { TechnologyTileDto } from "$dto/interfaces";
	import { interactiveElementClass } from "$utils/miscellanea";
	import { isNil, random } from "lodash";
	import ActionToken from "../ActionToken.svelte";
	import AdvancedTechnologyTile from "../AdvancedTechnologyTile.svelte";
	import StandardTechnologyTile from "../StandardTechnologyTile.svelte";

	export let tile: TechnologyTileDto;
	export let playerId: string;

	let clickable = random(true) > 0.5;
	let selected = random(true) > 0.75;

	$: covered = !isNil(tile.coveredByAdvancedTile);
	$: tileId = covered ? tile.coveredByAdvancedTile! : tile.id;
</script>

<div class="player-technology-tile w-full relative" class:interactive={clickable || selected}>
	<StandardTechnologyTile type={tile.id} />
	{#if tile.coveredByAdvancedTile}
		<div class="advanced-tile absolute">
			<AdvancedTechnologyTile type={tile.coveredByAdvancedTile} />
			<div class="action-token advanced">
				{#if tile.used}
					<ActionToken />
				{/if}
			</div>
		</div>
	{:else}
		<div class="action-token advanced">
			{#if tile.used}
				<ActionToken />
			{/if}
		</div>
	{/if}
	<div class={interactiveElementClass(clickable, selected)} style:pointer-events="all" on:click />
</div>

<style>
	.player-technology-tile:not(.interactive):hover .advanced-tile {
		transform: rotateX(90deg);
		transition: transform 250ms;
	}

	.advanced-tile {
		width: 82.5%;
		top: 8%;
		left: 5%;
	}

	.action-token {
		position: absolute;
		width: 50%;
		top: 20%;
		left: 25%;
	}

	.action-token.advanced {
		width: 64%;
		top: 5%;
		left: 10%;
	}
</style>
