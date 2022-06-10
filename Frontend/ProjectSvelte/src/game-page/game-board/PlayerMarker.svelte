<script lang="ts">
	import type { Race } from "$dto/enums";
	import { withAspectRatioW } from "$utils/miscellanea";
	import { getRaceColors } from "$utils/race-utils";

	export let race: Race;
	export let shape: "circle" | "octagon" = "circle";

	let style = "";
	$: {
		const [raceColor, borderColor] = getRaceColors(race);
		style = `
	${withAspectRatioW(1)};
	background-color: ${raceColor};
	border-color: ${borderColor}
	`;
	}
</script>

<div class="player-marker" {style} />

<style>
	.player-marker {
		width: calc(100% + 2px) !important;
		border: 1px solid;
		border-radius: 50%;
	}
</style>
