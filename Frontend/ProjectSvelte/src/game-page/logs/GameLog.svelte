<script lang="ts">
	import type { GameLogDto } from "$dto/interfaces";
	import PlayerLog from "./PlayerLog.svelte";
	import SystemLog from "./SystemLog.svelte";

	export let log: GameLogDto;
	export let canRollback: boolean;
	export let doRollback: (actionId: number) => void;
</script>

{#if log.isSystem}
	<SystemLog message={log.message} />
{:else}
	<PlayerLog {log} {canRollback} {doRollback} />
{/if}
