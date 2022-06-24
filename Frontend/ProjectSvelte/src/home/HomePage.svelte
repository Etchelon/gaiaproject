<script lang="ts">
	import Page from "$components/Page.svelte";
	import { getAppContext } from "../app/App.context";
	import AuthenticatedUserHome from "./AuthenticatedUserHome.svelte";
	import UnauthenticatedUserHome from "./UnauthenticatedUserHome.svelte";

	const { auth } = getAppContext();
	const { isAuthenticated } = auth;
</script>

<Page title="Star Gate">
	{#if $isAuthenticated}
		<AuthenticatedUserHome />
	{:else}
		<UnauthenticatedUserHome />
	{/if}
</Page>
