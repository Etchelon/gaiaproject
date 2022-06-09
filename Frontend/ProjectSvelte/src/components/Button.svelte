<script lang="ts">
	import { createEventDispatcher } from "svelte";

	export let size: "normal" | "small" = "normal";
	export let variant: "primary" | "default" = "default";
	export let disabled = false;

	const dispatch = createEventDispatcher();

	function handleClick(evt: Event) {
		console.log({ evt });
		evt.stopPropagation();
		dispatch("clicked");
	}

	let classes: string;
	$: {
		const sizeClasses = size === "normal" ? "py-2 px-4 text-sm" : "py-1 px-2 text-xs";
		const colorClasses = variant === "default" ? "bg-white text-gray-900 border-gray-400 border-2" : "bg-primary-700 text-white";
		classes = `${sizeClasses} ${colorClasses}`;
	}
</script>

<button type="button" class={`rounded-md hover:scale-105 ${classes}`} {disabled} on:click={handleClick}>
	<slot />
</button>
