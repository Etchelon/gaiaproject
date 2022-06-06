<script lang="ts">
	import { getAppContext } from "../App.context";
	import AuthenticatedUserMenu from "./AuthenticatedUserMenu.svelte";
	import UnauthenticatedUserMenu from "./UnauthenticatedUserMenu.svelte";

	export let contentId: string;
	const { auth } = getAppContext();
	const { isAuthenticated } = auth;
</script>

<ion-menu content-id={contentId} type="overlay" swipe-gesture>
	<ion-content>
		<ion-toolbar />
		<ion-menu-toggle auto-hide={false}>
			<ion-item href="#/" lines="full">
				<ion-label>
					<h2>Home</h2>
				</ion-label>
			</ion-item>
			{#if $isAuthenticated}
				<AuthenticatedUserMenu />
			{:else}
				<UnauthenticatedUserMenu />
			{/if}
		</ion-menu-toggle>
	</ion-content>
</ion-menu>
