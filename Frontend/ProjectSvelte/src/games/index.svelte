<script lang="ts" context="module">
	import type { GameKind } from "$utils/types";

	const gameKindLabels = new Map<GameKind, string>([
		["active", "Active Games"],
		["finished", "Finished Games"],
	]);
</script>

<script lang="ts">
	import Page from "$components/Page.svelte";
	import type { CreateGameCommand } from "$dto/interfaces";
	import { addOutline, caretForwardOutline, checkmarkDoneOutline } from "ionicons/icons";
	import { push, querystring } from "svelte-spa-router";
	import { getAppContext } from "../app/App.context";
	import NewGamePage from "../new-game/NewGamePage.svelte";
	import GamesList from "./GamesList.svelte";

	const { http } = getAppContext();

	let isCreating = false;
	const openModal = () => {
		isCreating = true;
	};
	const closeModal = () => {
		isCreating = false;
	};

	const startGame = async (command: CreateGameCommand) => {
		const gameId = await http.post<string>("api/GaiaProject/CreateGame", command, { readAsString: true });
		isCreating = false;
		push(`#/game/${gameId}`);
	};

	$: kind = (new URLSearchParams($querystring).get("kind") as GameKind | null) ?? "active";
	$: tabLabel = gameKindLabels.get(kind)!;
</script>

<Page title={tabLabel}>
	<GamesList {kind} />
	<ion-fab vertical="bottom" horizontal="end" slot="fixed">
		<ion-fab-button color="primary" on:click={openModal}>
			<ion-icon icon={addOutline} />
		</ion-fab-button>
	</ion-fab>
	<ion-tab-bar slot="footer">
		<ion-tab-button selected={kind === "active"} on:click={() => push("#/games")}>
			<ion-icon icon={caretForwardOutline} />
			<ion-label>Active</ion-label>
			<!-- TODO: always retrieve # of games where it's your turn <ion-badge>6</ion-badge> -->
		</ion-tab-button>

		<ion-tab-button selected={kind === "finished"} on:click={() => push("#/games?kind=finished")}>
			<ion-icon icon={checkmarkDoneOutline} />
			<ion-label>Finished</ion-label>
		</ion-tab-button>
	</ion-tab-bar>
</Page>

<ion-modal class="dialog" is-open={isCreating} on:didDismiss={closeModal}>
	<NewGamePage onStart={startGame} onClose={closeModal} />
</ion-modal>
