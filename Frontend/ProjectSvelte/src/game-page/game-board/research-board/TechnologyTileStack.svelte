<script lang="ts">
	import type { StandardTechnologyTileStackDto } from "$dto/interfaces";
	import StandardTechnologyTile from "../StandardTechnologyTile.svelte";

	export let stack: StandardTechnologyTileStackDto;
</script>

<div class="technology-tile-stack" class:invisible={stack.remaining === 0}>
	<StandardTechnologyTile type={stack.type} />
</div>

<style>
	.technology-tile-stack {
		position: relative;
		width: auto;
		height: auto;
	}

	.technology-tile-stack.invisible {
		visibility: hidden;
	}
</style>
