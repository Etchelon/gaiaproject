<script lang="ts">
	import { HttpClient } from "$utils/http-client";
	import { HubClient } from "$utils/hub-client";
	import { Capacitor } from "@capacitor/core";
	import { createClient } from "@supabase/supabase-js";
	import { onMount } from "svelte";
	import { AuthService } from "../auth/auth-service";
	import { setupIonic } from "../setup-ionic";
	import { IAppContext, initAppContext, PlatformType } from "./App.context";
	import App from "./App.svelte";

	const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
	const platform = Capacitor.getPlatform() as PlatformType;
	const supabase = createClient(import.meta.env.VITE_SUPABASE_URL, import.meta.env.VITE_SUPABASE_KEY);
	const http = new HttpClient(actualBaseUrl);
	const hub = new HubClient(actualBaseUrl);
	const auth = new AuthService(supabase, http, hub);
	const { isLoading } = auth;
	const ctx: IAppContext = {
		platform,
		supabase,
		http,
		hub,
		auth,
	};
	initAppContext(ctx);
	auth.initializeSupabase();

	onMount(async () => {
		setupIonic();
	});
</script>

{#if !$isLoading}
	<App />
{/if}
