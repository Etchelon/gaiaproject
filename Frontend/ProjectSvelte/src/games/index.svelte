<script lang="ts">
	import type { GameKind } from "$utils/types";
	import GamesPage from "./GamesPage.svelte";

	export let params: { kind: GameKind };
</script>

<GamesPage kind={params.kind} />
