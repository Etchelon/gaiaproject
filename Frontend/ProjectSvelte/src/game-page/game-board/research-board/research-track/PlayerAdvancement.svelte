<script lang="ts" context="module">
	const percentY = new Map<number, number>([
		[0, 87],
		[1, 75],
		[2, 63],
		[3, 44],
		[4, 32],
		[5, 5],
	]);
</script>

<script lang="ts">
	import type { Race } from "$dto/enums";
	import PlayerMarker from "../../PlayerMarker.svelte";

	export let race: Race;
	export let width: number;
	export let steps: number;

	$: style = `width: ${width * 0.6}px; top: ${percentY.get(steps) ?? 0}%; left: calc(50% - ${(width * 0.6) / 2}px)`;
</script>

<div class="player-advancement">
	<div class="marker-wrapper" {style}>
		<PlayerMarker {race} />
	</div>
</div>

<style>
	.player-advancement {
		position: relative;
		width: 100%;
		height: 100%;
	}

	.marker-wrapper {
		position: absolute;
	}
</style>
