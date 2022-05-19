<script lang="ts">
	import type { Race } from "$dto/enums";
	import { getRaceColor } from "$utils/race-utils";

	export let raceId: Race;
	export let width: number;

	$: style = `width: ${width}px; height: ${width}px; background-color: ${getRaceColor(raceId)}`;
</script>

<div class="satellite" {style} />

<style>
	.satellite {
		border: 2px solid darkgray;
		border-radius: 3px;
	}
</style>
