<script lang="ts">
	import Page from "$components/Page.svelte";
	import { setGamePageContext } from "./GamePage.context";
	import GamePage from "./GamePage.svelte";

	export let gameId: string;

	setGamePageContext(gameId);
</script>

<Page title="Game">
	<GamePage />
</Page>
