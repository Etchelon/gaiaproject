<script lang="ts">
	export let backgroundColor = "white";
	export let color = "black";
</script>

<li class={`p-3 text-xl text-${color} bg-${backgroundColor} list-none`}>
	<slot />
</li>
