<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";

	export let message: string;
</script>

<ListItem>
	<p class="m-0 gaia-font text-center">{message}</p>
</ListItem>
