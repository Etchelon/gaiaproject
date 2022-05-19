<script lang="ts" context="module">
	import type { ElementSize } from "$utils/types";

	const WIDTH_TO_HEIGHT_RATIO = 0.962;

	const roundTileCoordinates = new Map<number, { top: number; left: number; rotation: number }>([
		[1, { top: 0.255, left: 0.075, rotation: -74 }],
		[2, { top: 0.115, left: 0.159, rotation: -43 }],
		[3, { top: 0.038, left: 0.305, rotation: -14 }],
		[4, { top: 0.04, left: 0.4725, rotation: 15 }],
		[5, { top: 0.12, left: 0.618, rotation: 45 }],
		[6, { top: 0.26, left: 0.7, rotation: 75 }],
	]);

	const finalScoringTrackCoordinates = new Map<number, { top: number; left: number }>([
		[0, { top: 0.495, left: 0.06 }],
		[1, { top: 0.73, left: 0.06 }],
	]);

	const calculateHeight = (width: number) => {
		return width * WIDTH_TO_HEIGHT_RATIO;
	};

	const getRoundTileCoordinates = (round: number, width: number) => {
		const coordinates = roundTileCoordinates.get(round)!;
		return `top: ${coordinates.top * width}px; left: ${coordinates.left * width}px; transform: rotateZ(${coordinates.rotation}deg)`;
	};

	const getFinalScoringTrackCoordinates = (index: number, { width, height }: ElementSize) => {
		const coordinates = finalScoringTrackCoordinates.get(index)!;
		return `top: ${coordinates.top * height}px; left: ${coordinates.left * width}px`;
	};
</script>

<script lang="ts">
	import type { ScoringTrackDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { onMount } from "svelte";
	import RoundScoringTile from "./RoundScoringTile.svelte";
	import FinalScoringTrack from "./FinalScoringTrack.svelte";

	export let board: ScoringTrackDto;

	let container: HTMLDivElement;
	let width = 0;

	$: height = calculateHeight(width);
	$: roundTileWidth = width * 0.225;
	$: console.log({ height });

	onMount(() => {
		width = container.clientWidth;
		console.log({ width });
	});
</script>

<div class="scoring-track" style:height={`${height}px`} bind:this={container}>
	<img class="image" src={assetUrl("Boards/ScoreTrack.png")} alt="" />
	{#each board.scoringTiles as roundTile (roundTile.roundNumber)}
		<div class="round-tile" style={getRoundTileCoordinates(roundTile.roundNumber, width)}>
			<RoundScoringTile tile={roundTile} width={roundTileWidth} />
		</div>
	{/each}
	{#each [board.finalScoring1, board.finalScoring2] as finalScoring, index (finalScoring.tileId)}
		<div class="final-scoring" style={getFinalScoringTrackCoordinates(index, { width, height })}>
			<FinalScoringTrack scoring={finalScoring} />
		</div>
	{/each}
</div>

<style>
	.scoring-track {
		position: relative;
		width: 100%;
		background-color: black;
		border-width: 3;
		border-style: solid;
		border-color: lightgray;
		border-radius: 10;
	}

	.image {
		width: 100%;
		height: 100%;
		position: absolute;
		top: 0;
		left: 0;
	}

	.round-tile {
		position: absolute;
	}

	.final-scoring {
		position: absolute;
		width: 81%;
		height: auto;
	}
</style>
