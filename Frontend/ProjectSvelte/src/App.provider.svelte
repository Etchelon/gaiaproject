<script lang="ts">
	import { HttpClient } from "$utils/http-client";
	import { onMount } from "svelte";
	import { IAppContext, initAppContext } from "./App.context";
	import App from "./App.svelte";
	import { AuthService } from "./auth";
	import { setupIonic } from "./setup-ionic";

	const onRedirectCallback = (appState: any) => {
		window.history.replaceState({}, document.title, appState && appState.targetUrl ? appState.targetUrl : window.location.pathname);
	};

	const http = new HttpClient(import.meta.env.VITE_API_BASE_URL);
	const auth = new AuthService(http);
	const { isLoading } = auth;
	const ctx: IAppContext = {
		http,
		auth,
	};
	initAppContext(ctx);

	onMount(async () => {
		setupIonic();
		await auth.initializeAuth0(onRedirectCallback);
	});
</script>

{#if !$isLoading}
	<App />
{/if}
