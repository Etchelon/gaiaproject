<script lang="ts">
	export let contentId: string;
</script>

<div id={contentId} class="ion-page">
	<ion-header translucent>
		<ion-toolbar>
			<ion-buttons slot="start">
				<ion-menu-button />
			</ion-buttons>
			<ion-title class="gaia-font">Gaia Project</ion-title>
		</ion-toolbar>
	</ion-header>

	<ion-content fullscreen>
		<ion-header collapse="condense">
			<ion-toolbar>
				<ion-title class="gaia-font" size="large">Gaia Project</ion-title>
			</ion-toolbar>
		</ion-header>

		<slot />
	</ion-content>
</div>

<style>
	.ion-page {
		display: flex;
		flex-direction: column;
		height: 100%;
	}
</style>
