<script lang="ts">
	import Router, { replace } from "svelte-spa-router";
	import { get } from "svelte/store";
	import AppMenu from "../menu/AppMenu.svelte";
	import getRoutes from "../routes/index.mjs";
	import { getAppContext } from "./App.context";

	const { auth } = getAppContext();
	const { isAuthenticated } = auth;

	const onConditionFailed = () => {
		if (!get(isAuthenticated)) {
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
