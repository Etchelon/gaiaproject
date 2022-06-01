<script lang="ts">
	import Page from "$components/Page.svelte";
	import { setGamePageContext } from "./GamePage.context";
	import GamePage from "./GamePage.svelte";

	export let params: { id: string };

	setGamePageContext(params.id);
</script>

<Page title="Game">
	<GamePage />
</Page>
