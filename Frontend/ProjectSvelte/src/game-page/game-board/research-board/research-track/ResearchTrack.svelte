<script lang="ts">
	import type { ResearchTrackDto } from "$dto/interfaces";
	import { assetUrl, interactiveElementClass } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { isNil, noop } from "lodash";
	import { getGamePageContext } from "../../../GamePage.context";
	import { deriveResearchTrackInteractionState } from "../../../store/selectors";
	import AdvancedTechnologyTile from "../../AdvancedTechnologyTile.svelte";
	import FederationToken from "../../FederationToken.svelte";
	import TechnologyTileStack from "../TechnologyTileStack.svelte";
	import PlayerAdvancement from "./PlayerAdvancement.svelte";

	export let track: ResearchTrackDto;
	export let width: number;
	export let height: number;

	const { store, activeWorkflow } = getGamePageContext();
	const interactionState = deriveResearchTrackInteractionState(track.id)(store);
	$: ({ clickable, selected } = $interactionState);
	$: trackClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(track.id, InteractiveElementType.ResearchStep);
		  }
		: noop;
	$: style = `width: ${width}px; height: ${height}px`;
</script>

<div class="research-track" {style}>
	{#if !isNil(track.federation)}
		<div class="federation-token">
			<FederationToken type={track.federation} />
		</div>
	{/if}
	{#if track.lostPlanet}
		<img class="lostPlanetToken" src={assetUrl("Markers/LostPlanet.png")} alt="" />
	{/if}
	{#if track.advancedTileType}
		<div class="advanced-tile">
			<AdvancedTechnologyTile type={track.advancedTileType} />
		</div>
	{/if}
	<div class="player-markers">
		{#each track.players as playerAdvancement}
			<PlayerAdvancement width={width * 0.25} race={playerAdvancement.raceId} steps={playerAdvancement.steps} />
		{/each}
	</div>
	<div class="standard-tiles">
		<TechnologyTileStack stack={track.standardTiles} />
	</div>
	<div class={interactiveElementClass(clickable, selected)} on:click={trackClicked} />
</div>

<style>
	.research-track {
		position: relative;
		display: flex;
		flex-direction: column;
	}

	.federation-token {
		width: 32%;
		position: absolute;
		top: 1%;
		left: 38%;
	}

	.lostPlanetToken {
		position: absolute;
		top: 1%;
		left: 30%;
		height: 9%;
	}

	.advanced-tile {
		width: 78%;
		position: absolute;
		top: 10.5%;
		left: 19%;
	}

	.player-markers {
		display: flex;
		width: 100%;
		flex: 1 1 100%;
	}

	.standard-tiles {
		flex: 1 0 auto;
		align-self: center;
		width: 95%;
	}
</style>
