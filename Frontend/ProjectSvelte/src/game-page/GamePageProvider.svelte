<script lang="ts">
	import { setGamePageContext } from "./GamePage.context";
	import GamePage from "./GamePage.svelte";

	export let gameId: string;

	setGamePageContext(gameId);
</script>

<GamePage />
