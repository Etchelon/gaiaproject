<script lang="ts">
	import { gameDto } from "./data";
	import Map from "./game-page/game-board/map/Map.svelte";
	import ResearchBoard from "./game-page/game-board/research-board/ResearchBoard.svelte";
	import ScoringBoard from "./game-page/game-board/scoring-board/ScoringBoard.svelte";

	export let name = "Project Svelte";
	let playAreaWidth = 500;
</script>

<main>
	<h1 class="gaia-font text-primary-600">Hello {name}!</h1>
	<section>
		<input type="range" bind:value={playAreaWidth} min="500" max={window.innerWidth - 16} />
	</section>
	<ScoringBoard
		board={gameDto.boardState.scoringBoard}
		federationTokens={gameDto.boardState.availableFederations}
		roundBoosters={gameDto.boardState.availableRoundBoosters}
	/>
	<ResearchBoard board={gameDto.boardState.researchBoard} width={playAreaWidth} />
	<Map map={gameDto.boardState.map} width={playAreaWidth} />
</main>

<style>
	h1 {
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}
</style>
