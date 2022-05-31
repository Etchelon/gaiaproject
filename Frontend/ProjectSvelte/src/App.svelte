<script lang="ts">
	import Page from "$components/Page.svelte";
	import { onMount } from "svelte";
	import GamePageProvider from "./game-page/GamePageProvider.svelte";
	import { setupIonic } from "./setup-ionic";

	onMount(setupIonic);
</script>

<ion-app>
	<ion-split-pane content-id="main-content">
		<ion-menu content-id="main-content" type="reveal" swipe-gesture>
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
	</ion-split-pane>
	<Page contentId="main-content">
		<GamePageProvider gameId="" />
	</Page>
</ion-app>
