<script lang="ts">
	import { logIn, logOut } from "ionicons/icons";
	import { onMount } from "svelte";
	import { get } from "svelte/store";
	import Router, { replace } from "svelte-spa-router";
	import type { ConditionsFailedEvent } from "svelte-spa-router";
	import { initAppContext } from "./App.context";
	import routes from "./routes";
	import { setupIonic } from "./setup-ionic";
	import { useAuth0 } from "./auth";
	import checkFirstLogin from "./auth/checkFirstLogin";

	const { isAuthenticated, user, login, logout, initializeAuth0 } = useAuth0;

	const onConditionFailed = (event: ConditionsFailedEvent) => {
		if (!get(isAuthenticated)) {
			replace("/unauthorized");
			return;
		}

		replace("/not-found");
	};

	const onRedirectCallback = (appState: any) => {
		window.history.replaceState({}, document.title, appState && appState.targetUrl ? appState.targetUrl : window.location.pathname);
	};

	initAppContext();

	onMount(async () => {
		setupIonic();
		await initializeAuth0(onRedirectCallback);
	});

	$: {
		if ($isAuthenticated && $user) {
			checkFirstLogin();
		}
	}
</script>

<ion-app>
	<ion-split-pane content-id="main-content" when="(min-width: 2000px)">
		<ion-menu content-id="main-content" type="overlay" swipe-gesture>
			<ion-content>
				<!-- <div class="toolbar-spacer" /> -->
				<ion-toolbar />
				<ion-menu-toggle auto-hide={false}>
					<ion-item href="#/" lines="full">
						<ion-label>
							<h2>Home</h2>
						</ion-label>
					</ion-item>
					<ion-item href="#/games" lines="full">
						<ion-label>
							<h2>Games</h2>
							<p>Your Games</p>
						</ion-label>
					</ion-item>
					{#if !$isAuthenticated}
						<ion-item lines="full">
							<ion-label>
								<h2>Anonymous user</h2>
								<p>Login to use all the features</p>
							</ion-label>
							<ion-button slot="end" fill="none" on:click={login}>
								<ion-icon slot="icon-only" icon={logIn} />
							</ion-button>
						</ion-item>
					{:else}
						<ion-item lines="full">
							<ion-avatar slot="start">
								<ion-img src="user.avatar" alt="AVT" />
							</ion-avatar>
							<ion-label>
								<h2>{$user?.nickname}</h2>
							</ion-label>
							<ion-button slot="end" fill="none" on:click={logout}>
								<ion-icon slot="icon-only" icon={logOut} />
							</ion-button>
						</ion-item>
					{/if}
				</ion-menu-toggle>
			</ion-content>
		</ion-menu>
		<div id="main-content" class="contents">
			<Router {routes} on:conditionsFailed={onConditionFailed} />
		</div>
	</ion-split-pane>
</ion-app>

<style>
	ion-split-pane {
		--side-max-width: 300px;
	}

	.toolbar-spacer {
		width: 100%;
		height: 56px;
	}
</style>
