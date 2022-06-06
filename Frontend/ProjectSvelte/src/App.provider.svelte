<script lang="ts">
	import { once } from "lodash";
	import { onMount } from "svelte";
	import { initAppContext } from "./App.context";
	import App from "./App.svelte";
	import { useAuth0 } from "./auth";
	import checkFirstLogin from "./auth/checkFirstLogin";
	import { setupIonic } from "./setup-ionic";

	const { isAuthenticated, isLoading, user, initializeAuth0 } = useAuth0;

	const onRedirectCallback = (appState: any) => {
		window.history.replaceState({}, document.title, appState && appState.targetUrl ? appState.targetUrl : window.location.pathname);
	};

	const onAuthenticated = once(checkFirstLogin);

	initAppContext();

	onMount(async () => {
		setupIonic();
		await initializeAuth0(onRedirectCallback);
	});

	$: {
		if ($isAuthenticated && $user) {
			onAuthenticated();
		}
	}
</script>

{#if !$isLoading}
	<App />
{/if}
