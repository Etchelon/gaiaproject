<script lang="ts">
	import Page from "$components/Page.svelte";
	import { getAppContext } from "../App.context";

	const {
		auth: { loggedUser },
	} = getAppContext();
</script>

<Page title="Profile">
	<h1>Profile page for user {$loggedUser.username}</h1>
</Page>
