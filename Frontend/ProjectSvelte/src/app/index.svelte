<script lang="ts">
	import { HttpClient } from "$utils/http-client";
	import { HubClient } from "$utils/hub-client";
	import { onMount } from "svelte";
	import { IAppContext, initAppContext, PlatformType } from "./App.context";
	import App from "./App.svelte";
	import { AuthServiceApp } from "../auth/auth-service.app";
	import { AuthServiceWeb } from "../auth/auth-service.web";
	import { setupIonic } from "../setup-ionic";
	import { Capacitor } from "@capacitor/core";

	const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
	const platform = Capacitor.getPlatform() as PlatformType;
	const http = new HttpClient(actualBaseUrl);
	const hub = new HubClient(actualBaseUrl);
	const auth = platform === "web" ? new AuthServiceWeb(http, hub) : new AuthServiceApp(http, hub);
	const { isLoading } = auth;
	const ctx: IAppContext = {
		platform,
		http,
		hub,
		auth,
	};
	initAppContext(ctx);
	auth.initializeAuth0();

	onMount(async () => {
		setupIonic();
	});
</script>

{#if !$isLoading}
	<App />
{/if}
