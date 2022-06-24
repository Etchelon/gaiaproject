<script lang="ts" context="module">
	const BOOSTER_HEIGHT_TO_WIDTH_RATIO = 3;
	const BOOSTER_AND_FEDERATION_WIDTH = 70;
	const BOOSTER_SPACING = 5;
	const FEDERATION_WIDTH = 70;
	const FEDERATION_SPACING = 5;
</script>

<script lang="ts">
	import type { GameStateDto } from "$dto/interfaces";
	import { ActiveView } from "$utils/types";
	import FederationTokenStack from "../game-board/scoring-board/FederationTokenStack.svelte";
	import Map from "../game-board/map/Map.svelte";
	import ResearchBoard from "../game-board/research-board/ResearchBoard.svelte";
	import RoundBooster from "../game-board/RoundBooster.svelte";
	import ScoringTrack from "../game-board/scoring-board/scoring-track/ScoringTrack.svelte";
	import { selectAllRoundBoosters } from "$utils/selectors";
	import TurnOrderMinimap from "../turn-order/TurnOrderMinimap.svelte";

	export let game: GameStateDto;
	export let width: number;
	export let height: number;
	export let showMinimaps: boolean;
	export let minimapClicked: (view: ActiveView) => void;

	$: map = game.boardState.map;
	$: boosters = selectAllRoundBoosters(game);
	$: nBoosters = boosters.length;
	$: federations = game.boardState.availableFederations.filter(stack => stack.remaining > 0);
	$: nFederations = federations.length;
</script>

<div class="main-view flex justify-center items-center overflow-auto relative wh-full">
	{#if showMinimaps}
		<div class="minimap scoring-track">
			<ScoringTrack board={game.boardState.scoringBoard} width={width / 4} />
			<div class="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)} />
		</div>
		<div class="minimap research-board zoomable">
			<ResearchBoard board={game.boardState.researchBoard} width={width * 0.3} height={height * 0.3} />
			<div class="click-trap" onClick={() => minimapClicked(ActiveView.ResearchBoard)} />
		</div>
		<div class="minimap boosters-and-federations">
			<div class="round-boosters" style:width={`${nBoosters * BOOSTER_AND_FEDERATION_WIDTH + (nBoosters - 1) * BOOSTER_SPACING}px`}>
				{#each boosters as booster, index (booster.id)}
					<div
						class="round-booster"
						style:width={`${BOOSTER_AND_FEDERATION_WIDTH}px`}
						style:right={`${(BOOSTER_AND_FEDERATION_WIDTH + BOOSTER_SPACING) * index}px`}
					>
						<RoundBooster {booster} withPlayerInfo={true} nonInteractive={true} />
					</div>
				{/each}
				<div class="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)} />
			</div>
			<div
				class="federations"
				style:width={`${nFederations * FEDERATION_WIDTH + (nFederations - 1) * FEDERATION_SPACING}px`}
				style:top={`calc(${BOOSTER_HEIGHT_TO_WIDTH_RATIO * BOOSTER_AND_FEDERATION_WIDTH}px + 8px)`}
			>
				{#each federations as stack, index (stack.type)}
					<div
						class="federation"
						style:width={`${FEDERATION_WIDTH}px`}
						style:right={`${(FEDERATION_WIDTH + FEDERATION_SPACING) * index}px`}
					>
						<FederationTokenStack {stack} />
					</div>
				{/each}
				<div class="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)} />
			</div>
		</div>
		<div class="minimap turn-order">
			<TurnOrderMinimap {game} direction="vertical" />
		</div>
	{/if}
	<div class="flex justify-center wh-full overflow-auto">
		<Map {map} {height} />
	</div>
</div>

<style lang="scss">
	.main-view {
		background-color: black;
	}

	.minimap {
		position: absolute;
		z-index: 0;

		&:hover {
			z-index: 3;
		}

		&.zoomable:hover {
			zoom: 2;
		}

		& > .click-trap {
			position: absolute;
			top: 0;
			left: 0;
			right: 0;
			bottom: 0;
			pointer-events: all;
		}
	}

	.scoring-track {
		top: 0;
		left: 0;
	}

	.research-board {
		bottom: 0;
		left: 0;
	}

	.boosters-and-federations {
		top: 0;
		right: 0;
		width: 70px;
	}

	.round-boosters {
		display: flex;
		justify-content: flex-end;
		overflow: hidden;
		transition: width 0.25s;
		position: absolute;
		right: 0;

		&:not(:hover) {
			width: 70px !important;
		}

		.round-booster {
			position: absolute;
			top: 0;

			&:first-child {
				position: static;
			}
		}
	}

	.federations {
		display: flex;
		justify-content: flex-end;
		overflow: hidden;
		transition: width 0.25s;
		position: absolute;
		right: 0;

		&:not(:hover) {
			width: 70px !important;
		}

		.federation {
			position: absolute;
			bottom: 0;

			&:first-child {
				position: static;
			}
		}
	}

	.turn-order {
		bottom: 0;
		right: 0;
	}
</style>
