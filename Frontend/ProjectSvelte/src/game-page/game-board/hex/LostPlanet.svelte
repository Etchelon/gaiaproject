<script lang="ts">
	import type { Race } from "../../../dto/enums";
	import { assetUrl } from "../../../utils/miscellanea";
	import Satellite from "./Satellite.svelte";

	export let raceId: Race;
	export let width: number;
</script>

<div class="root">
	<img class="lost-planet" src={assetUrl("Markers/LostPlanet.png")} alt="" />
	<div class="satellite-wrapper">
		<Satellite {raceId} width={width / 5} />
	</div>
</div>

<style>
	.root {
		position: relative;
		width: 100%;
		height: 100%;
	}

	.lost-planet {
		position: absolute;
		width: 75%;
		left: 12.5%;
		top: 7.5%;
	}

	.satellite-wrapper {
		position: absolute;
		right: 20%;
		bottom: 20%;
	}
</style>
