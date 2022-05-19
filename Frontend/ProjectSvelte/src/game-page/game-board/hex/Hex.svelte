<script lang="ts" context="module">
	import { chain } from "lodash";
	import { hexHeight, hexSide } from "../shape-utils";

	interface HexStyles {
		width: number;
		height: number;
		top: number;
		left: number;
	}

	interface HexSizingProps {
		index: number;
		width: number;
		height: number;
		side: number;
	}

	const getRowAndColumn = (index: number) => {
		const indexMinus3 = index - 3;
		const normalizedTo5 = indexMinus3 % 5;
		const row =
			3 <= index && index <= 15
				? 2 + (normalizedTo5 / 2 <= 1 ? 0 : 1) + 2 * parseInt(String((index - 3) / 5), 10) // replace with Math.floor?
				: index === 0
				? 0
				: index === 18
				? 8
				: index === 1 || index === 2
				? 1
				: index === 16 || index === 17
				? 7
				: 0;
		const column =
			3 <= index && index <= 15
				? normalizedTo5 < 3
					? 2 * (normalizedTo5 % 3)
					: 3 - 2 * (normalizedTo5 % 2)
				: index === 0 || index === 18
				? 2
				: index === 1 || index === 16
				? 1
				: index === 2 || index === 17
				? 3
				: 0;
		return { row, column };
	};

	const getX = (index: number, width: number, side: number) => {
		const { row, column } = getRowAndColumn(index);
		const isEvenRow = row % 2 === 0;
		return isEvenRow ? (column / 2) * (width + side) : (width + side) / 2 + ((column - 1) / 2) * (side + width);
	};

	const getY = (index: number, height: number) => {
		const { row } = getRowAndColumn(index);
		return (height / 2) * row;
	};

	const getHexDimensions = (width: number) => {
		const height = hexHeight(width);
		const side = hexSide(width);
		return { height, side };
	};

	const hexCorners = ({ width, height, side }: HexSizingProps) => {
		const points = [
			{ x: (width - side) / 2, y: 0 },
			{ x: (width + side) / 2, y: 0 },
			{ x: width, y: height / 2 },
			{ x: (width + side) / 2, y: height },
			{ x: (width - side) / 2, y: height },
			{ x: 0, y: height / 2 },
		];
		return chain(points)
			.map(p => `${p.x},${p.y}`)
			.value()
			.join(" ");
	};
</script>

<script lang="ts">
	import { isEmpty, isNil } from "lodash";
	import { BuildingType, PlanetType, Race } from "../../../dto/enums";
	import type { HexDto } from "../../../dto/interfaces";
	import { assetUrl } from "../../../utils/miscellanea";
	import Building from "./Building.svelte";
	import Satellite from "./Satellite.svelte";
	import LostPlanet from "./LostPlanet.svelte";
	import { random } from "lodash";
	import Selector from "./Selector.svelte";

	export let hex: HexDto;
	export let width = 100;
	export let onClicked = () => {
		console.info(`Hex ${hex.id} clicked!!`);
	};

	let clickable = random(true) > 0.5;
	let selected = random(true) > 0.75;
	let notes = random(true) > 0.85;

	let hexStyles: HexStyles;
	let hexSizing: HexSizingProps;
	$: {
		const index = hex.index;
		const { height, side } = getHexDimensions(width);
		hexStyles = {
			width,
			height,
			top: getY(index, height),
			left: getX(index, width, side),
		};
		hexSizing = {
			width,
			height,
			index,
			side,
		};
	}
	$: hasGaiaMarker = hex.planetType === PlanetType.Transdim && hex.wasGaiaformed;
	$: hasLostPlanet = hex.planetType === PlanetType.LostPlanet;
	$: hasIvitsSpaceStation = !!hex.ivitsSpaceStation;
	$: hasBuilding = !isNil(hex.building);
	$: hasFederationMarker = hex.building?.showFederationMarker ?? false;
	$: hasLantidsMine = !isNil(hex.lantidsParasiteBuilding);
	$: hasSatellites = !isEmpty(hex.satellites);
</script>

<div class="hex" style:width={`${hexStyles.width}px`} style:height={`${hexStyles.height}px`} style:top={`${hexStyles.top}px`} style:left={`${hexStyles.left}px`}>
	{#if hasGaiaMarker}
		<img class="gaia-marker" src={assetUrl("/GaiaMarker.png")} alt="" />
	{/if}
	{#if hasIvitsSpaceStation}
		<img class="gaia-marker" src={assetUrl("/Markers/SpaceStation.png")} alt="" />
	{/if}
	{#if hasLostPlanet}
		<div class="building-wrapper">
			<LostPlanet raceId={hex.building.raceId} {width} />
		</div>
	{/if}
	{#if hasBuilding && hex.building.type !== BuildingType.LostPlanet}
		<div class="building-wrapper">
			<Building raceId={hex.building.raceId} type={hex.building.type} onMap />
			{#if hasFederationMarker}
				<img class="federation-marker" src={assetUrl("Markers/RecordToken.png")} alt="" />
			{/if}
		</div>
	{/if}
	{#if hasLantidsMine}
		<div class="lantids-mine">
			<Building raceId={Race.Lantids} type={BuildingType.Mine} onMap />
		</div>
	{/if}
	{#if hasSatellites}
		<div class="satellites">
			{#each hex.satellites as satellite}
				<Satellite raceId={satellite.raceId} width={width / 5} />
			{/each}
		</div>
	{/if}
	{#if clickable || selected}
		<div class="selector-wrapper">
			<Selector radius={hexSizing.height} {selected} withQic={notes} />
		</div>
	{/if}
	<svg class="clicker">
		<polygon class:clickable points={hexCorners(hexSizing)} onClick={onClicked} />
	</svg>
</div>

<style>
	.hex {
		position: absolute;
		pointer-events: none;
	}

	.hex > :not(.clicker) {
		position: absolute;
	}

	.gaia-marker {
		width: 75%;
		left: 12.5%;
		top: 7.5%;
	}

	.building-wrapper {
		z-index: 1;
		width: 100%;
		height: 100%;
	}

	.federation-marker {
		position: absolute;
		width: 35%;
		bottom: 10%;
		right: 15%;
	}

	.lantids-mine {
		width: 60%;
		height: 60%;
		right: 0;
		bottom: 0;
		z-index: 2;
	}

	.satellites {
		display: flex;
		flex-flow: row wrap;
		align-items: center;
		justify-content: space-around;
		gap: 5px;
		width: 100%;
		height: 100%;
		padding: 15%;
		box-sizing: border-box;
	}

	.selector-wrapper {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100%;
		height: 100%;
	}

	.clicker {
		width: 100%;
		height: 100%;
	}

	.clicker polygon {
		fill: transparent;
		stroke: rgb(239, 239, 239, 0.75);
		stroke-width: 1;
	}

	.clicker polygon.clickable {
		cursor: pointer;
		pointer-events: all;
	}
</style>
