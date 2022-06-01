<script lang="ts">
	import { logIn } from "ionicons/icons";
	import { onMount } from "svelte";
	import Router from "svelte-spa-router";
	import routes from "./routes";
	import { setupIonic } from "./setup-ionic";

	onMount(setupIonic);
</script>

<ion-app>
	<ion-split-pane content-id="main-content" when="(min-width: 2000px)">
		<ion-menu content-id="main-content" type="overlay" swipe-gesture>
			<ion-content>
				<ion-menu-toggle auto-hide={false}>
					<ion-item href="#/" lines="full">
						<ion-label>
							<h2>Home</h2>
						</ion-label>
					</ion-item>
					<ion-item href="#/games" lines="full">
						<ion-label>
							<h2>Games</h2>
							<p>Your active games</p>
						</ion-label>
						<ion-button slot="end" fill="none" on:click={() => {}}>
							<ion-icon slot="icon-only" icon={logIn} />
						</ion-button>
					</ion-item>
				</ion-menu-toggle>
			</ion-content>
		</ion-menu>
		<div id="main-content" class="contents">
			<Router {routes} />
		</div>
	</ion-split-pane>
</ion-app>

<style>
	ion-split-pane {
		--side-max-width: 300px;
	}
</style>
