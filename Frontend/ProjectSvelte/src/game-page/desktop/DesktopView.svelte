<script lang="ts" context="module">
	import { ActiveView } from "$utils/types";

	const ACTIVATABLE_VIEWS = [
		{ label: "Map", view: ActiveView.Map },
		{ label: "Research Board", view: ActiveView.ResearchBoard },
		{ label: "Scoring Board", view: ActiveView.ScoringBoard },
	];
</script>

<script lang="ts">
	import type { GameStateDto, PlayerInGameDto } from "$dto/interfaces";
	import { noop } from "lodash";
	import { onMount } from "svelte";
	import PlayerArea from "../game-board/players/PlayerArea.svelte";
	import PlayerBox from "../game-board/players/PlayerBox.svelte";
	import ResearchBoard from "../game-board/research-board/ResearchBoard.svelte";
	import ScoringBoard from "../game-board/scoring-board/ScoringBoard.svelte";
	import GameLog from "../logs/GameLog.svelte";
	import MainView from "./MainView.svelte";

	export let game: GameStateDto;
	export let players: PlayerInGameDto[];
	export let activeView: ActiveView;
	export let currentPlayerId = "";
	export let isSpectator = false;

	let container: HTMLDivElement;
	let width: number;
	let height: number;
	onMount(() => {
		width = container.clientWidth;
		height = container.clientHeight;
		console.log({ container });
	});

	const setActiveView = (view: ActiveView) => {
		activeView = view;
	};

	let playerAreaToShow: PlayerInGameDto | null = null;

	$: actualView = activeView === ActiveView.Map || activeView === ActiveView.PlayerArea ? ActiveView.Map : activeView;
</script>

<div class="w-full h-screen p-2 overflow-x-hidden overflow-y-auto bg-gray-900">
	<div class="grid grid-cols-4 gap-2 h-full">
		<div class="col-span-3 h-full flex flex-col gap-2">
			<div class="flex justify-center items-center flex-auto relative" bind:this={container}>
				{#if actualView === ActiveView.Map}
					<MainView {game} {width} {height} showMinimaps={true} minimapClicked={noop} />
				{:else if actualView === ActiveView.ResearchBoard}
					<ResearchBoard board={game.boardState.researchBoard} {width} {height} />
				{:else if actualView === ActiveView.ScoringBoard}
					<ScoringBoard
						board={game.boardState.scoringBoard}
						roundBoosters={game.boardState.availableRoundBoosters}
						federationTokens={game.boardState.availableFederations}
						isMobile={false}
					/>
				{:else if actualView === ActiveView.NotesAndSettings}
					<!-- <PlayerConfig gameId={game.id} /> -->
				{/if}
				{#if playerAreaToShow}
					<div class="absolute z-10 bg-gray-900">
						<PlayerArea player={playerAreaToShow} framed />
					</div>
				{/if}
			</div>
			<div class="tabs flex items-center justify-center gap-4 flex-shrink-0 bg-white">
				{#each ACTIVATABLE_VIEWS as av (av.view)}
					<div
						class="tab text-center cursor-pointer gaia-font"
						class:active={actualView === av.view}
						on:click={() => setActiveView(av.view)}
					>
						{av.label}
					</div>
				{/each}
			</div>
		</div>
		<div class="h-full overflow-x-hidden overflow-y-auto">
			<div class="w-full flex flex-col gap-2">
				{#each game.players as player, index (player.id)}
					<PlayerBox {player} {index} />
				{/each}
			</div>
			<div class="w-full mt-2 flex flex-col gap-2">
				{#each game.gameLogs as log}
					<GameLog {log} canRollback={false} doRollback={noop} />
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
