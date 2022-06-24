<script lang="ts" context="module">
	import { hexSpike, tileHeight, tileWidth } from "./shape-utils";

	const getX = (column: number, hexWidth: number) => {
		const columnWidth = hexWidth - hexSpike(hexWidth);
		return columnWidth * column;
	};

	const getY = (row: number, hexHeight: number) => {
		const rowHeight = hexHeight / 2;
		return rowHeight * row;
	};

	const getSectorWidth = (hexWidth: number) => {
		return tileWidth(hexWidth);
	};

	const getSectorHeight = (hexWidth: number, hexHeight: number) => {
		return tileHeight(hexWidth, hexHeight);
	};
</script>

<script lang="ts">
	import type { SectorDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import Hex from "./hex/Hex.svelte";

	export let sector: SectorDto;
	export let hexWidth: number;
	export let hexHeight: number;
	export let xOffset: number;
	export let yOffset: number;

	$: style = `
	width: ${getSectorWidth(hexWidth)}px;
	height: ${getSectorHeight(hexWidth, hexHeight)}px;
	top: ${getY(sector.row, hexHeight) + yOffset}px;
	left: ${getX(sector.column, hexWidth) + xOffset}px;
	`;
	$: imgRotation = `rotateZ(-${sector.rotation * 60}deg)`;
</script>

<div class="sector" {style}>
	<img class="image" style:transform={imgRotation} src={assetUrl(`Sectors/${sector.id}.png`)} alt="" />
	{#each sector.hexes as hex (hex.id)}
		<Hex {hex} width={hexWidth} />
	{/each}
</div>

<style>
	.sector {
		position: absolute;
		overflow: hidden;
		pointer-events: none;
	}

	.image {
		position: absolute;
		width: 100%;
		height: 100%;
		top: 0;
		left: 0;
	}
</style>
