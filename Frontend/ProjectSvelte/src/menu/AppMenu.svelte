<script lang="ts">
	import { getAppContext } from "../app/App.context";
	import AuthButton from "../auth/AuthButton.svelte";
	import AuthenticatedUserMenu from "./AuthenticatedUserMenu.svelte";
	import UnauthenticatedUserMenu from "./UnauthenticatedUserMenu.svelte";

	export let contentId: string;
	const { auth } = getAppContext();
	const { isAuthenticated } = auth;
</script>

<ion-menu content-id={contentId} type="overlay" swipe-gesture>
	<ion-menu-toggle class="flex flex-col h-full" auto-hide={false}>
		<ion-header>
			<ion-toolbar class="flex-shrink-0">
				<ion-title class="gaia-font w-full px-2">
					<a href="#/"> Gaia Project </a>
				</ion-title>
			</ion-toolbar>
		</ion-header>
		{#if $isAuthenticated}
			<AuthenticatedUserMenu />
		{:else}
			<UnauthenticatedUserMenu />
		{/if}
		<ion-footer>
			<ion-toolbar>
				<AuthButton />
			</ion-toolbar>
		</ion-footer>
	</ion-menu-toggle>
</ion-menu>
