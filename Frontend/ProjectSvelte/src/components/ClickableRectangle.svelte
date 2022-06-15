<script lang="ts">
	export let active: boolean;
	export let style: string;
	export let absolute = true;
</script>

<div class="rectangle border-2 rounded-md pointer-events-none" class:absolute class:active {style} on:click />

<style>
	.rectangle {
		border-color: transparent;
	}

	.active {
		border-color: var(--selectable-color);
		pointer-events: all;
		cursor: pointer;
	}
</style>
