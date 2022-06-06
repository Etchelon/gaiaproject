<script lang="ts">
	import Router, { replace } from "svelte-spa-router";
	import { getAppContext } from "./App.context";
	import AppMenu from "./menu/AppMenu.svelte";
	import getRoutes from "./routes/index.mjs";

	const { auth } = getAppContext();
	const { isAuthenticated, loggedUser, login, logout } = auth;

	const onConditionFailed = () => {
		if (!$isAuthenticated) {
			replace("/unauthorized");
			return;
		}

		replace("/not-found");
	};

	const routes = getRoutes(auth);
</script>

<ion-app>
	<ion-split-pane content-id="main-content" when="(min-width: 2000px)">
		<AppMenu contentId="main-content" />
		<div id="main-content" class="contents">
			<Router {routes} on:conditionsFailed={onConditionFailed} />
		</div>
	</ion-split-pane>
</ion-app>

<style>
	ion-split-pane {
		--side-max-width: 300px;
	}
</style>
