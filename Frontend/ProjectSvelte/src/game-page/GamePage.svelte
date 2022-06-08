<script lang="ts" context="module">
	export const GAMEVIEW_WRAPPER_ID = "gameViewWrapper";
	export const STATUSBAR_ID = "statusBar";
</script>

<script lang="ts">
	import Page from "$components/Page.svelte";
	import { actionSheetController } from "@ionic/core/components";
	import type { IonModal } from "@ionic/core/components/ion-modal";
	import { airplane } from "ionicons/icons";
	import { onMount } from "svelte";
	import DesktopView from "./desktop/DesktopView.svelte";
	import { getGamePageContext } from "./GamePage.context";
	import MobileView from "./mobile/MobileView.svelte";
	import StatusBar from "./status-bar/StatusBar.svelte";

	async function showActionSheet() {
		console.log({ actionSheetController, airplane });
		const actionSheet = await actionSheetController.create({
			buttons: [
				{
					text: "Yay",
					icon: airplane,
				},
			],
		});
		await actionSheet.present();
	}

	const { store } = getGamePageContext();
	const { activeView, currentPlayer, game, players } = store;
	let isMobile = false;

	const checkIsMobile = () => {
		isMobile = window.innerWidth <= 600;
	};
	onMount(checkIsMobile);

	let modal: IonModal;
	let showModal = false;
</script>

<svelte:window on:resize={checkIsMobile} />

{#if $game}
	<Page title={$game.name}>
		<div id={GAMEVIEW_WRAPPER_ID} class="game-page h-full bg-gray-900">
			<div id={STATUSBAR_ID} class="status-bar" class:desktop={!isMobile} class:mobile={isMobile}>
				<StatusBar playerId={null} {isMobile} />
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

		<ion-modal
			bind:this={modal}
			is-open={showModal}
			on:click={async () => {
				await modal.dismiss();
				showModal = false;
			}}
		>
			<h1 on:click={showActionSheet}>Hello Ionic modal!</h1>
		</ion-modal>
	</Page>
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
</style>
