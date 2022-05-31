<script lang="ts">
	import Page from "$components/Page.svelte";
	import { onMount } from "svelte";
	import GamePageProvider from "./game-page/GamePageProvider.svelte";
	import { setupIonic } from "./setup-ionic";

	onMount(setupIonic);
</script>

<ion-app>
	<ion-split-pane content-id="main-content" when="(min-width: 2000px)">
		<ion-menu content-id="main-content" type="overlay" swipe-gesture>
			<ion-content>
				<ion-menu-toggle auto-hide={false}>
					<ion-item lines="full">
						<ion-label>
							<h2>Games</h2>
							<p>Your active games</p>
						</ion-label>
						<ion-button slot="end" fill="none" on:click={() => {}}>
							<ion-icon slot="icon-only" name="log-in" />
						</ion-button>
					</ion-item>
				</ion-menu-toggle>
			</ion-content>
		</ion-menu>
		<Page contentId="main-content">
			<GamePageProvider gameId="" />
		</Page>
	</ion-split-pane>
</ion-app>

<style>
	ion-split-pane {
		--side-max-width: 300px;
	}
</style>
