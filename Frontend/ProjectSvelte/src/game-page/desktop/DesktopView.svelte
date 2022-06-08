<script lang="ts" context="module">
	import { ActiveView } from "$utils/types";

	const PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO = 1.439;
	const ACTIVATABLE_VIEWS = [
		{ label: "Map", view: ActiveView.Map },
		{ label: "Research Board", view: ActiveView.ResearchBoard },
		{ label: "Scoring Board", view: ActiveView.ScoringBoard },
	];
</script>

<script lang="ts">
	import { GamePhase } from "$dto/enums";
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { noop } from "lodash";
	import { onMount } from "svelte";
	import PlayerArea from "../game-board/players/PlayerArea.svelte";
	import PlayerBox from "../game-board/players/PlayerBox.svelte";
	import ResearchBoard from "../game-board/research-board/ResearchBoard.svelte";
	import ScoringBoard from "../game-board/scoring-board/ScoringBoard.svelte";
	import GameLog from "../logs/GameLog.svelte";
	import MainView from "./MainView.svelte";
	import { getGamePageContext } from "../GamePage.context";

	export let currentPlayerId = "";
	export let isSpectator = false;

	const { store } = getGamePageContext();
	const { activeView, game } = store;

	$: players = $game.players;
	$: isGameCreator = $game.createdBy.id === currentPlayerId;
	$: canRollback = isGameCreator && $game.currentPhase === GamePhase.Rounds;
	$: actualView = $activeView === ActiveView.Map || $activeView === ActiveView.PlayerArea ? ActiveView.Map : activeView;
	$: {
		if ($activeView !== ActiveView.PlayerArea) {
			hidePlayerArea();
		} else {
			showPlayerArea(currentPlayerId);
		}
	}

	//#region Sizing

	let container: HTMLDivElement;
	let width: number;
	let height: number;
	let playerAreaModalStyle = `width: 0; height: 0; top: 0; left: 0;`;

	const calculatePlayerAreaModalSize = () => {
		width = container.clientWidth;
		height = container.clientHeight;

		let dpaHeight = height * 0.95;
		let dpaWidth = dpaHeight * PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		if (dpaWidth > width) {
			dpaWidth = width * 0.95;
			dpaHeight = dpaWidth / PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		}

		playerAreaModalStyle = `width: ${dpaWidth}px; height: ${dpaHeight}px; top: ${(height - dpaHeight) / 2}px; left: ${
			(width - dpaWidth) / 2
		}px;`;
	};

	onMount(() => {
		const resizeObserver = new ResizeObserver(calculatePlayerAreaModalSize);
		resizeObserver.observe(container);

		return () => resizeObserver.disconnect();
	});

	//#endregion

	let playerAreaToShow: PlayerInGameDto | null = null;

	const showPlayerArea = (playerId: string) => {
		if (playerId === playerAreaToShow?.id) {
			hidePlayerArea();
			return;
		}

		const player = players.find(p => p.id === playerId)!;
		playerAreaToShow = player;
	};
	const hidePlayerArea = () => {
		playerAreaToShow = null;
	};
</script>

<div class="desktop-view wh-full overflow-x-hidden overflow-y-auto">
	<div class="grid grid-cols-4 gap-2 h-full">
		<div class="col-span-3 h-full flex flex-col gap-2">
			<div class="flex justify-center items-center flex-auto relative" bind:this={container}>
				{#if actualView === ActiveView.Map}
					<MainView game={$game} {width} {height} showMinimaps={true} minimapClicked={noop} />
				{:else if actualView === ActiveView.ResearchBoard}
					<ResearchBoard board={$game.boardState.researchBoard} {width} {height} />
				{:else if actualView === ActiveView.ScoringBoard}
					<ScoringBoard
						board={$game.boardState.scoringBoard}
						roundBoosters={$game.boardState.availableRoundBoosters}
						federationTokens={$game.boardState.availableFederations}
					/>
				{:else if actualView === ActiveView.NotesAndSettings}
					<!-- <PlayerConfig gameId={game.id} /> -->
				{/if}
				{#if playerAreaToShow}
					<div class="absolute z-10 bg-gray-900" style={playerAreaModalStyle}>
						<PlayerArea player={playerAreaToShow} framed />
					</div>
				{/if}
			</div>
			<div class="tabs flex items-center justify-center gap-4 flex-shrink-0 bg-white">
				{#each ACTIVATABLE_VIEWS as av (av.view)}
					<div
						class="tab text-center text-gray-900 cursor-pointer gaia-font"
						class:active={actualView === av.view}
						on:click={() => store.setActiveView(av.view)}
					>
						{av.label}
					</div>
				{/each}
			</div>
		</div>
		<div class="h-full overflow-x-hidden overflow-y-auto">
			<div class="w-full flex flex-col gap-2">
				{#each $game.players as player, index (player.id)}
					<div class="contents" on:click={() => showPlayerArea(player.id)}>
						<PlayerBox {player} {index} />
					</div>
				{/each}
			</div>
			<div class="w-full mt-2 flex flex-col gap-2">
				{#each $game.gameLogs as log}
					<GameLog {log} {canRollback} doRollback={noop} />
				{/each}
			</div>
		</div>
	</div>
</div>

<style>
	.tabs {
		height: 40px;
	}

	.tab {
		border-bottom: 0 solid black;
	}

	.tab.active {
		border-bottom-width: 3px;
	}
</style>
