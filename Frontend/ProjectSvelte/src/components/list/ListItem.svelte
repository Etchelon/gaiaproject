<script lang="ts">
	export let noPadding = false;
</script>

<div class={`wrapper flex items-center text-xl ${noPadding ? "" : " py-2 px-4"} hover:cursor-pointer`}>
	<slot />
</div>

<style>
	:root {
		--background-color: white;
		--color: black;
	}

	.wrapper {
		background-color: var(--background-color);
		color: var(--color);
	}
</style>
