<script lang="ts" context="module">
	import { FinalScoringTileType } from "$dto/enums";

	const WIDTH_TO_HEIGHT_RATIO = 4.85;
	const TILE_WIDTH_TO_HEIGHT_RATIO = 1.6;

	const finalScoringTileNames = new Map<FinalScoringTileType, string>([
		[FinalScoringTileType.BuildingsInAFederation, "FINfed"],
		[FinalScoringTileType.BuildingsOnTheMap, "FINbld"],
		[FinalScoringTileType.KnownPlanetTypes, "FINtyp"],
		[FinalScoringTileType.GaiaPlanets, "FINgai"],
		[FinalScoringTileType.Sectors, "FINsec"],
		[FinalScoringTileType.Satellites, "FINsat"],
	]);
</script>

<script lang="ts">
	import type { FinalScoringStateDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import PlayerFinalScoringAdvancement from "./PlayerFinalScoringAdvancement.svelte";

	export let scoring: FinalScoringStateDto;
	export let width: number;

	$: height = width / WIDTH_TO_HEIGHT_RATIO;
	$: style = `width: ${width}px; height: ${height}px`;
</script>

<div class="final-scoring-track" {style}>
	<div class="wrapper fill-parent">
		<div class="player-scorings" {style}>
			{#each scoring.players as playerAdvancement (playerAdvancement.player.id)}
				<div class="player-scoring">
					<PlayerFinalScoringAdvancement playerStatus={playerAdvancement} />
				</div>
			{/each}
		</div>
		<div class="tile" style:width={`${height * TILE_WIDTH_TO_HEIGHT_RATIO}px`}>
			<img class="fill-parent" src={assetUrl(`Boards/FinalScoring/${finalScoringTileNames.get(scoring.tileId)}.png`)} alt="" />
		</div>
	</div>
</div>

<style>
	.final-scoring-track {
		width: 100%;
	}

	.wrapper {
		display: flex;
	}

	.player-scorings {
		flex: 1 1 auto;
		display: flex;
		flex-direction: column;
		justify-content: space-around;
	}

	.player-scorings > .player-scoring {
		height: 22.5%;
	}

	.tile {
		height: 100%;
		flex: 0 0 auto;
		margin-left: auto;
	}
</style>
