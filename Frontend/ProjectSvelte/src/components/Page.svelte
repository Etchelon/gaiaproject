<script lang="ts">
	import { getAppContext } from "../app/App.context";
	import Notifications from "../notifications/index.svelte";

	export let title: string;

	const { auth } = getAppContext();
	const { isAuthenticated } = auth;
</script>

<div class="ion-page">
	<ion-header translucent>
		<ion-toolbar>
			<ion-buttons slot="start">
				<ion-menu-button />
			</ion-buttons>
			<ion-title class="gaia-font">{title}</ion-title>
			{#if $isAuthenticated}
				<ion-buttons slot="end">
					<Notifications />
				</ion-buttons>
			{/if}
		</ion-toolbar>
	</ion-header>

	<ion-content fullscreen>
		<ion-header collapse="condense">
			<ion-toolbar>
				<ion-title class="gaia-font" size="large">{title}</ion-title>
			</ion-toolbar>
		</ion-header>

		<slot />
		<slot name="fixed" />
	</ion-content>

	<slot name="footer" />
</div>
