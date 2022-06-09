<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";

	const gameKindLabels = new Map<GameKind, string>([
		["active", "Active Games"],
		["waiting", "Waiting for You"],
		["finished", "Finished Games"],
	]);
</script>

<script lang="ts">
	import Page from "$components/Page.svelte";
	import GamesPage from "./GamesPage.svelte";

	export let params: { kind: GameKind };
	const { kind } = params;
</script>

<Page title={gameKindLabels.get(kind) ?? ""}>
	<GamesPage kind={params.kind} />
</Page>
