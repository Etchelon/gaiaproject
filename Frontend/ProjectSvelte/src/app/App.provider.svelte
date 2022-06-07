<script lang="ts">
	import { HttpClient } from "$utils/http-client";
	import { HubClient } from "$utils/hub-client";
	import { onMount } from "svelte";
	import { IAppContext, initAppContext } from "./App.context";
	import App from "./App.svelte";
	import { AuthService } from "../auth";
	import { setupIonic } from "../setup-ionic";

	const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
	const http = new HttpClient(actualBaseUrl);
	const hub = new HubClient(actualBaseUrl);
	const auth = new AuthService(http);
	const { isLoading } = auth;
	const ctx: IAppContext = {
		http,
		hub,
		auth,
	};
	initAppContext(ctx);

	onMount(async () => {
		setupIonic();
		await auth.initializeAuth0();
	});
</script>

{#if !$isLoading}
	<App />
{/if}
