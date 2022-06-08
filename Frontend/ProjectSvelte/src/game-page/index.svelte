<script lang="ts">
	import { noop } from "lodash";
	import { onDestroy, onMount } from "svelte";
	import { getAppContext } from "../app/App.context";
	import { GamePageSignalRConnectionService } from "./GamePageSignalRConnection.service";
	import { IGamePageContext, setGamePageContext } from "./GamePage.context";
	import { GamePageStore } from "./store/GamePage.store";
	import GamePage from "./GamePage.svelte";

	export let params: { id: string };

	const { id } = params;
	const { http, hub } = getAppContext();
	const store = new GamePageStore(http);
	const signalR = new GamePageSignalRConnectionService(hub, store, noop);
	const ctx: IGamePageContext = {
		id,
		store,
		signalR,
		activeWorkflow: null,
	};

	setGamePageContext(ctx);

	onMount(async () => {
		await store.loadGame(id);
		await signalR.connectToHub();
	});

	onDestroy(async () => {
		await signalR.disconnectFromHub();
	});
</script>

<GamePage />
