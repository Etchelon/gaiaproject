<script lang="ts">
	export let active: boolean;
	export let style: string;
</script>

<div class="border-2 rounded-md pointer-events-none" class:active {style} on:click />

<style>
	.active {
		border-color: var(--selectable-color);
		pointer-events: all;
		cursor: pointer;
	}
</style>
