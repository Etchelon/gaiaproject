<script lang="ts">
	export let title: string;
</script>

<div class="ion-page">
	<ion-header translucent>
		<ion-toolbar>
			<ion-buttons slot="start">
				<ion-menu-button />
			</ion-buttons>
			<ion-title class="gaia-font">{title}</ion-title>
		</ion-toolbar>
	</ion-header>

	<ion-content fullscreen>
		<ion-header collapse="condense">
			<ion-toolbar>
				<ion-title class="gaia-font" size="large">{title}</ion-title>
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
