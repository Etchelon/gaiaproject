<script lang="ts">
	import { setGamePageContext } from "./GamePage.context";
	import GamePage from "./GamePage.svelte";

	export let params: { id: string };

	setGamePageContext(params.id);
</script>

<GamePage />
