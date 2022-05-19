<script lang="ts">
	import { BuildingType, PlanetType, Race } from "./dto/enums";

	import type { HexDto } from "./dto/interfaces";
	import Hex from "./game-page/game-board/hex/Hex.svelte";

	export let name = "Project Svelte";
	let hexes: HexDto[] = [
		<any>{
			id: "hex-42",
			index: 0,
			building: <any>{
				type: BuildingType.PlanetaryInstitute,
				raceId: Race.Lantids,
				showFederationMarker: true,
			},
		},
		<any>{
			id: "hex-43",
			index: 1,
			planetType: PlanetType.Transdim,
			wasGaiaformed: true,
			building: <any>{
				type: BuildingType.TradingStation,
				raceId: Race.Ivits,
				showFederationMarker: false,
			},
			lantidsParasiteBuilding: {
				type: BuildingType.Mine,
				raceId: Race.Lantids,
			},
		},
		<any>{
			id: "hex-44",
			index: 2,
			building: <any>{
				type: BuildingType.Mine,
				raceId: Race.Xenos,
				showFederationMarker: false,
			},
		},
		{
			id: "hex-45",
			index: 3,
			satellites: [
				{
					type: BuildingType.Satellite,
					raceId: Race.Xenos,
				},
				{
					type: BuildingType.Satellite,
					raceId: Race.Ivits,
				},
			],
		},
		{
			id: "hex-46",
			index: 4,
			satellites: [
				{
					type: BuildingType.Satellite,
					raceId: Race.Xenos,
				},
				{
					type: BuildingType.Satellite,
					raceId: Race.Ivits,
				},
				{
					type: BuildingType.Satellite,
					raceId: Race.Lantids,
				},
			],
		},
		{
			id: "hex-47",
			index: 5,
			planetType: PlanetType.LostPlanet,
			building: {
				type: BuildingType.LostPlanet,
				raceId: Race.Xenos,
			},
		},
		{
			id: "hex-48",
			index: 6,
			ivitsSpaceStation: {
				raceId: Race.Ivits,
				type: BuildingType.IvitsSpaceStation,
			},
		},
	];
</script>

<main>
	<h1 class="gaia-font">Hello {name}!</h1>
	<div class="sector">
		{#each hexes as hex}
			<Hex {hex} />
		{/each}
	</div>
</main>

<style>
	main {
		text-align: center;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}

	.sector {
		margin: 2rem;
		position: relative;
	}
</style>
