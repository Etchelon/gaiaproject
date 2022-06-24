<script lang="ts">
	import type { TechnologyTileDto } from "$dto/interfaces";
	import { interactiveElementClass } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { isNil, noop } from "lodash";
	import { get } from "svelte/store";
	import { getGamePageContext } from "../../GamePage.context";
	import { deriveOwnAdvancedTileInteractionState, deriveOwnStandardTileInteractionState } from "../../store/selectors";
	import ActionToken from "../ActionToken.svelte";
	import AdvancedTechnologyTile from "../AdvancedTechnologyTile.svelte";
	import StandardTechnologyTile from "../StandardTechnologyTile.svelte";

	export let tile: TechnologyTileDto;
	export let playerId: string;

	const { store, activeWorkflow } = getGamePageContext();

	let clickable = false;
	let selected = false;
	let tileClicked = noop;
	$: {
		const covered = !isNil(tile.coveredByAdvancedTile);
		const tileId = covered ? tile.coveredByAdvancedTile! : tile.id;
		const selector = covered
			? deriveOwnAdvancedTileInteractionState(tile.coveredByAdvancedTile!)
			: deriveOwnStandardTileInteractionState(tile.id);
		const interactionState = selector(store, playerId);
		const state = get(interactionState);
		clickable = state.clickable;
		selected = state.selected;
		tileClicked = clickable
			? () => {
					$activeWorkflow?.elementSelected(
						tileId,
						covered ? InteractiveElementType.OwnAdvancedTile : InteractiveElementType.OwnStandardTile
					);
			  }
			: noop;
	}
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
	<div class={interactiveElementClass(clickable, selected)} style:pointer-events="all" on:click|stopPropagation={tileClicked} />
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
