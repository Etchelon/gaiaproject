<script lang="ts">
	import { gameDto } from "./data";
	import Map from "./game-page/game-board/map/Map.svelte";
	import ResearchBoard from "./game-page/game-board/research-board/ResearchBoard.svelte";

	export let name = "Project Svelte";
	let playAreaWidth = 500;
</script>

<main>
	<h1 class="gaia-font">Hello {name}!</h1>
	<section>
		<input type="range" bind:value={playAreaWidth} min="500" max={window.innerWidth} />
	</section>
	<ResearchBoard board={gameDto.boardState.researchBoard} width={playAreaWidth} />
	<Map map={gameDto.boardState.map} width={playAreaWidth} />
</main>

<style>
	main {
		text-align: center;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>
