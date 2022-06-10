<script lang="ts" context="module">
	const getCubeX = (count: number): string => `${count === 0 ? 1 : 8.85 * (count - 1) + 11}%`;
</script>

<script lang="ts">
	import type { PlayerFinalScoringStatusDto } from "$dto/interfaces";
	import { getRaceColors } from "$utils/race-utils";

	export let playerStatus: PlayerFinalScoringStatusDto;

	let cubeStyle: string;
	$: {
		const [color, borderColor] = getRaceColors(playerStatus.player?.raceId);
		cubeStyle = `border-color: ${borderColor}; background-color: ${color}; `;
	}
</script>

<div class="player-final-scoring-advancement wh-full">
	<div class="cube" style={`${cubeStyle}; left: ${getCubeX(playerStatus.count > 10 ? 0 : playerStatus.count)}`} />
	{#if playerStatus.count > 10}
		<div class="cube" style={`${cubeStyle}; left: ${getCubeX(playerStatus.count - 10)}`} />
	{/if}
</div>

<style>
	.player-final-scoring-advancement {
		position: relative;
	}

	.player-final-scoring-advancement > .cube {
		position: absolute;
		width: calc(6% + 2px); /* 2px is twice the border */
		padding-top: 6%;
		top: 0;
		border: 1px solid;
		border-radius: 3px;
	}
</style>
