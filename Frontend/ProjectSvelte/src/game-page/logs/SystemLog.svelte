<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";

	export let message: string;
</script>

<ListItem>
	<ListItemText classes="text-center" text={message} />
</ListItem>
