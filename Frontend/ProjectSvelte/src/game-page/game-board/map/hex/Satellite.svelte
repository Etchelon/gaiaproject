<script lang="ts">
	import type { Race } from "$dto/enums";
	import { getRaceColors } from "$utils/race-utils";

	export let raceId: Race;
	export let width: number;

	$: style = `width: ${width}px; height: ${width}px; background-color: ${getRaceColors(raceId)[0]}`;
</script>

<div class="satellite" {style} />

<style>
	.satellite {
		border: 2px solid darkgray;
		border-radius: 3px;
	}
</style>
