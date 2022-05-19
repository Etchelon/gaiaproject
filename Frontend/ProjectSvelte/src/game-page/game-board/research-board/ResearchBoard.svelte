<script lang="ts" context="module">
	import type { ElementSize } from "$utils/types";
	import { isNil } from "lodash";

	const HEIGHT_TO_WIDTH_RATIO = 0.932;
	const freeTilesLeftMargins = ["7%", "17%", "15.5%"];

	const calculateSize = (parentWidth: number, parentHeight?: number): ElementSize => {
		if (!isNil(parentHeight)) {
			const widthFromHeight = parentHeight / HEIGHT_TO_WIDTH_RATIO;
			if (widthFromHeight <= parentWidth) {
				return { width: widthFromHeight, height: parentHeight };
			}
		}

		const heightFromWidth = parentWidth * HEIGHT_TO_WIDTH_RATIO;
		return { width: parentWidth, height: heightFromWidth };
	};
</script>

<script lang="ts">
	import type { ResearchBoardDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import ActionSpace from "../ActionSpace.svelte";
	import ResearchTrack from "./research-track/ResearchTrack.svelte";

	export let board: ResearchBoardDto;
	export let width: number;
	export let height: number | undefined = undefined;

	$: size = calculateSize(width, height);
	$: style = `width: ${size.width}px; height: ${size.height}px`;
	$: trackWidth = (size.width * 0.975) / 6;
	$: trackHeight = size.height * 0.725;
</script>

<div class="research-board" {style}>
	<img class="image" src={assetUrl("Boards/ResearchBoard.jpg")} alt="" />
	<div class="tracks">
		{#each board.tracks as track (track.id)}
			<ResearchTrack {track} width={trackWidth} height={trackHeight} />
		{/each}
	</div>
	<div class="free-tiles">
		{#each board.freeStandardTiles as stack, index (stack.type)}
			<div class="free-tile" style:margin-left={freeTilesLeftMargins[index]}>
				<!-- <TechnologyTileStack stack={stack} /> -->
			</div>
		{/each}
	</div>
	<div class="actions">
		<div class="power-actions">
			{#each board.powerActions as pa (`${pa.kind}_${pa.type}`)}
				<div class="power-action">
					<ActionSpace space={pa} />
				</div>
			{/each}
		</div>
		<div class="qic-actions">
			{#each board.qicActions as qa (`${qa.kind}_${qa.type}`)}
				<div class="qic-action">
					<ActionSpace space={qa} />
				</div>
			{/each}
		</div>
	</div>
</div>

<style>
	.research-board {
		position: relative;
		background-color: #efefef;
	}

	.image {
		width: 100%;
		height: 100%;
		position: absolute;
		top: 0;
		left: 0;
	}

	.tracks {
		display: flex;
		width: 100%;
		height: 72.5%;
		padding-left: 0.5%;
		position: absolute;
		top: 0;
		left: 0;
	}

	.free-tiles {
		display: flex;
		width: 100%;
		height: 14%;
		padding: 0.6%;
		position: absolute;
		top: 72.5%;
		left: 0;
	}

	.free-tile {
		width: 16%;
	}

	.actions {
		display: flex;
		width: 100%;
		height: 13.5%;
		padding: 0 2% 0 1.8%;
		position: absolute;
		top: 86.5%;
		left: 0;
	}

	.power-actions {
		display: flex;
		justify-content: space-between;
		width: 64%;
		height: 100%;
	}

	.power-action {
		width: calc(100% / 7 - 2px);
		height: 100%;
		position: relative;
	}

	.qic-actions {
		display: flex;
		width: 36%;
		height: 100%;
	}

	.qic-action {
		width: calc(100% / 3);
		height: 100%;
		position: relative;
	}
</style>
