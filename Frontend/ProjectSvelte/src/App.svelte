<script lang="ts">
	import { onMount } from "svelte";

	import { gameDto } from "./data";
	import MainView from "./game-page/desktop/MainView.svelte";
	import Map from "./game-page/game-board/map/Map.svelte";
	import PlayerArea from "./game-page/game-board/players/PlayerArea.svelte";
	import PlayerBox from "./game-page/game-board/players/PlayerBox.svelte";
	import ResearchBoard from "./game-page/game-board/research-board/ResearchBoard.svelte";
	import ScoringBoard from "./game-page/game-board/scoring-board/ScoringBoard.svelte";
	import GameLog from "./game-page/logs/GameLog.svelte";
	import StatusBar from "./game-page/status-bar/StatusBar.svelte";

	export let name = "Project Svelte";
	let playAreaWidth: number;

	onMount(() => {
		playAreaWidth = window.innerWidth - 16;
		console.log({ playAreaWidth });
	});

	$: isMobile = playAreaWidth < 600;
</script>

<main>
	<div class="desktop-view">
		<MainView game={gameDto} width={playAreaWidth} height={(playAreaWidth * 3) / 4} showMinimaps minimapClicked={() => {}} />
	</div>
	<div class="status-bar-wrapper" class:mobile={isMobile}>
		<StatusBar game={gameDto} playerId="" {isMobile} isSpectator={false} />
	</div>
	<h1 class="gaia-font text-primary-600">Hello {name}!</h1>
	<section>
		<input type="range" bind:value={playAreaWidth} min="500" max={window.innerWidth - 16} />
	</section>
	<hr />
	<div class="container flex">
		<div class="player-boxes-and-logs w-full md:w-1/2">
			{#each gameDto.players as player, index (player.id)}
				<PlayerBox {player} {index} />
			{/each}
		</div>
		<div class="player-boxes-and-logs w-full md:w-1/2">
			{#each gameDto.gameLogs as log}
				<div class="mb-2 last:mb-0">
					<GameLog {log} canRollback={false} doRollback={() => {}} />
				</div>
			{/each}
		</div>
	</div>
	<ScoringBoard
		board={gameDto.boardState.scoringBoard}
		federationTokens={gameDto.boardState.availableFederations}
		roundBoosters={gameDto.boardState.availableRoundBoosters}
	/>
	<ResearchBoard board={gameDto.boardState.researchBoard} width={playAreaWidth} />
	<Map map={gameDto.boardState.map} width={playAreaWidth} />
	{#each gameDto.players as player}
		<div class="player-area-wrapper" style:width={`${playAreaWidth}px`}>
			<PlayerArea {player} />
		</div>
	{/each}
</main>

<style>
	.status-bar-wrapper {
		height: 56px;
		padding: 3px;
	}

	.status-bar-wrapper.mobile {
		position: sticky;
		top: 0;
		height: auto;
		padding: 3px 0;
		z-index: 11;
	}

	h1 {
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}

	.player-boxes-and-logs {
		max-width: 350px;
		padding: 1rem;
		background-color: black;
	}
</style>
