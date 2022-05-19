<script lang="ts">
	import type { Race } from "$dto/enums";
	import { getContrastColor } from "$utils/colors";
	import { getRaceColor } from "$utils/race-utils";

	export let race: Race;
	export let shape: "circle" | "octagon" = "circle";

	let style = "";
	$: {
		const raceColor = getRaceColor(race);
		style = `
	background-color: ${raceColor};
	border-color: ${getContrastColor(raceColor)}
	`;
	}
</script>

<div class="player-marker wth-ratio-1" {style} />

<style>
	.player-marker {
		width: calc(100% + 2px) !important;
		border: 1px solid;
		border-radius: 50%;
	}
</style>
