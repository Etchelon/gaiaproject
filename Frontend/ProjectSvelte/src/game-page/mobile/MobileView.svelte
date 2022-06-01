<script lang="ts" context="module">
	import { ActiveView } from "$utils/types";

	const TOOLBAR_HEIGHT = 48;
	const viewsAnchors = new Map<ActiveView, string>([
		[ActiveView.Map, "map"],
		[ActiveView.ResearchBoard, "researchBoard"],
		[ActiveView.ScoringBoard, "scoringBoard"],
		[ActiveView.PlayerArea, "boxesAndLogs"],
	]);
</script>

<script lang="ts">
	import { GamePhase } from "$dto/enums";
	import { noop } from "lodash";
	import { onMount } from "svelte";
	import GameMap from "../game-board/map/Map.svelte";
	import { getGamePageContext } from "../GamePage.context";
	import { GAMEVIEW_WRAPPER_ID, STATUSBAR_ID } from "../GamePage.svelte";
	import ResearchBoard from "../game-board/research-board/ResearchBoard.svelte";
	import ScoringBoard from "../game-board/scoring-board/ScoringBoard.svelte";
	import TurnOrderMinimap from "../turn-order/TurnOrderMinimap.svelte";
	import PlayerBoxOrArea from "./PlayerBoxOrArea.svelte";
	import GameLog from "../logs/GameLog.svelte";

	export let currentPlayerId = "";
	export let isSpectator = false;

	const { store } = getGamePageContext();

	$: game = $store.game;
	$: map = game.boardState.map;
	$: players = game.players;
	$: isGameCreator = game.createdBy.id === currentPlayerId;
	$: canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;
	$: activeView = $store.activeView;
	$: {
		const elementId = viewsAnchors.get(activeView) ?? "";
		const element = document.getElementById(elementId);
		if (element) {
			const gameViewWrapper = document.getElementById(GAMEVIEW_WRAPPER_ID)!;
			const statusBar = document.getElementById(STATUSBAR_ID)!;
			const top = element.offsetTop - TOOLBAR_HEIGHT - 3 - statusBar.clientHeight - 3; // 3px spacing below the toolbar and status bar
			gameViewWrapper.scrollTo({ top, behavior: "smooth" });
		}
	}

	//#region Sizing

	let container: HTMLDivElement;
	let width: number;

	onMount(() => {
		width = container.clientWidth;
	});

	//#endregion
</script>

<div bind:this={container}>
	<div id="map" class="w-full overflow-x-auto overflow-y-hidden">
		<GameMap {map} {width} />
	</div>
	<div class="w-full mt-2" />
	<div id="researchBoard">
		<ResearchBoard board={game.boardState.researchBoard} {width} />
	</div>
	<div class="w-full mt-2" />
	<div id="scoringBoard">
		<ScoringBoard
			board={game.boardState.scoringBoard}
			roundBoosters={game.boardState.availableRoundBoosters}
			federationTokens={game.boardState.availableFederations}
			isMobile={true}
		/>
	</div>
	<div class="w-full mt-2" />
	<div id="turnOrder">
		<TurnOrderMinimap {game} direction="horizontal" />
	</div>
	<div class="w-full mt-2" />
	<div id="boxesAndLogs" class="h-full overflow-x-hidden overflow-y-auto">
		<div class="w-full flex flex-col gap-2 relative">
			{#each game.players as player, index (player.id)}
				<div class="contents">
					<PlayerBoxOrArea
						{player}
						{index}
						forcePlayerAreaView={player.id === currentPlayerId && activeView === ActiveView.PlayerArea}
					/>
				</div>
			{/each}
		</div>
		<div class="w-full mt-2 flex flex-col gap-2">
			{#each game.gameLogs as log}
				<GameLog {log} {canRollback} doRollback={noop} />
			{/each}
		</div>
	</div>
</div>
