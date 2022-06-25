<script lang="ts" context="module">
	import { assetUrl } from "$utils/miscellanea";
	export const STATUSBAR_ID = "statusBar";

	const playersTurnAudioUrl = assetUrl("Sounds/PlayersTurn.wav");
</script>

<script lang="ts">
	import { ActiveView, isDialogView } from "$utils/types";
	import { App } from "@capacitor/app";
	import { Capacitor, PluginListenerHandle } from "@capacitor/core";
	import { chain, isNil, noop, size } from "lodash";
	import { onDestroy, onMount } from "svelte";
	import { get } from "svelte/store";
	import { getAppContext } from "../app/App.context";
	import DesktopView from "./desktop/DesktopView.svelte";
	import AuctionDialog from "./dialogs/auction/AuctionDialog.svelte";
	import ConversionsDialog from "./dialogs/conversions/ConversionsDialog.svelte";
	import SelectRaceDialog from "./dialogs/select-race/SelectRaceDialog.svelte";
	import SortIncomesDialog from "./dialogs/sort-incomes/SortIncomesDialog.svelte";
	import TerransDecideIncomeDialog from "./dialogs/terrans-decide-income/TerransDecideIncomeDialog.svelte";
	import { getGamePageContext } from "./GamePage.context";
	import MobileView from "./mobile/MobileView.svelte";
	import StatusBar from "./status-bar/StatusBar.svelte";
	import type { ActionWorkflow } from "./workflows/action-workflow.base";
	import { fromAction, fromDecision } from "./workflows/utils";

	const { platform, auth } = getAppContext();
	const { store, signalR, activeWorkflow, startWorkflow } = getGamePageContext();
	const { activeView, currentPlayer, game, isSpectator, players } = store;
	let appStateChangeListener: PluginListenerHandle;
	let isMobile = false;

	const checkIsMobile = () => {
		isMobile = Capacitor.getPlatform() !== "web" || window.innerWidth <= 600;
	};

	onMount(async () => {
		checkIsMobile();
		if (!get(auth.isAuthenticated)) {
			return;
		}

		await signalR.connectToHub();
		appStateChangeListener = await App.addListener("appStateChange", async ({ isActive }) => {
			if (isActive) {
				await signalR.connectToHub();
			} else {
				platform !== "web" && isMobile && (await signalR.disconnectFromHub());
			}
		});
	});

	onDestroy(async () => {
		await signalR.disconnectFromHub();
		await appStateChangeListener.remove();
	});

	$: {
		(() => {
			// With the adjust-sectors action I'm introducing a new feature: a workflow that dispatches
			// changes to the store. This causes game to be updated, which triggers this effect
			// but in this case all changes are temporary and client side so the workflow will manage
			// everything, we don't need to run the effect
			if (!isNil($activeWorkflow)) {
				return;
			}

			store.clearStatus();

			const gameIsOver = !isNil($game.ended);
			if (gameIsOver) {
				const winnersNames = chain($game.players)
					.filter(p => p.placement === 1)
					.map(p => p.username)
					.value();
				const message = `Game over. ${winnersNames.join(", ")} won!`;
				store.setStatusMessage(message);
				return;
			}

			const activePlayer = $game.activePlayer;
			if (!activePlayer) {
				return;
			}

			const isActivePlayer = activePlayer.id === $currentPlayer?.id;
			if ($isSpectator || !isActivePlayer) {
				activeWorkflow.set(null);
				store.setStatusMessage(activePlayer.reason);
				return;
			}

			if (!isMobile && isActivePlayer) {
				const audio = new Audio(playersTurnAudioUrl);
				audio.volume = 0.5;
				audio.play().catch(noop);
			}
			let workflow: ActionWorkflow | null = null;
			if (activePlayer.pendingDecision) {
				workflow = fromDecision($currentPlayer!.id, $game, activePlayer.pendingDecision);
			} else if (size(activePlayer.availableActions) === 1) {
				workflow = fromAction($currentPlayer!.id, $game, activePlayer.availableActions[0], store);
			}

			if (!workflow) {
				store.setWaitingForAction(activePlayer.availableActions);
				return;
			}

			startWorkflow(workflow);
		})();
	}
</script>

<svelte:window on:resize={checkIsMobile} />

<div class="game-page bg-gray-900" class:h-full={!isMobile}>
	<div id={STATUSBAR_ID} class="status-bar" class:desktop={!isMobile} class:mobile={isMobile}>
		<StatusBar {isMobile} />
	</div>
	<div class="game-view" class:desktop={!isMobile} class:mobile={isMobile}>
		{#if isMobile}
			<MobileView game={$game} players={$players} activeView={$activeView} currentPlayerId={$currentPlayer?.id} />
		{:else}
			<DesktopView
				game={$game}
				players={$players}
				activeView={$activeView}
				currentPlayerId={$currentPlayer?.id}
				on:setActiveView={evt => store.setActiveView(evt.detail)}
			/>
		{/if}
	</div>
</div>

{#if !$isSpectator && $currentPlayer}
	<ion-modal class="dialog" class:mobile={isMobile} is-open={isDialogView($activeView)}>
		{#if $activeView === ActiveView.RaceSelectionDialog}
			<SelectRaceDialog gameId={$game.id} />
		{/if}
		{#if $activeView === ActiveView.AuctionDialog}
			<AuctionDialog gameId={$game.id} />
		{/if}
		{#if $activeView === ActiveView.ConversionDialog}
			<ConversionsDialog gameId={$game.id} currentPlayer={$currentPlayer} />
		{/if}
		{#if $activeView === ActiveView.SortIncomesDialog}
			<SortIncomesDialog gameId={$game.id} currentPlayer={$currentPlayer} />
		{/if}
		{#if $activeView === ActiveView.TerransConversionsDialog}
			<TerransDecideIncomeDialog gameId={$game.id} currentPlayer={$currentPlayer} />
		{/if}
	</ion-modal>
{/if}

<style lang="scss">
	.status-bar {
		padding: 3px;

		&.desktop {
			height: 56px;
		}

		&.mobile {
			position: sticky;
			top: 0;
			padding-left: 0;
			padding-right: 0;
			z-index: 11;
			background-color: black;
		}
	}

	.game-view {
		padding: 3px;

		&.desktop {
			height: calc(100% - 56px);
			overflow: hidden;
		}

		&.mobile {
			overflow: auto;
		}
	}

	.dialog:not(.mobile) {
		--width: 75vw;
		--height: 75vh;
	}
</style>
