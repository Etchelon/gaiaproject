<script lang="ts">
	import type { AvailableActionDto } from "$dto/interfaces";
	import { get } from "svelte/store";
	import { getGamePageContext } from "../GamePage.context";
	import type { Command } from "../workflows/types";
	import { fromAction } from "../workflows/utils";

	export let isMobile: boolean;

	const { store, activeWorkflow, startWorkflow } = getGamePageContext();
	const { availableActions, availableCommands, currentPlayer, game, isExecutingAction, isSpectator, statusMessage } = store;

	let showMenu = false;
	const openMenu = () => {
		showMenu = true;
	};
	const closeMenu = () => {
		showMenu = false;
	};

	const handleCommand = (command: Command) => {
		const wf = get(activeWorkflow);
		if (!wf) {
			throw new Error("How can you issue a command without an active workflow?!");
		}

		const action = wf.handleCommand(command);
		if (!action) {
			return;
		}

		store.executePlayerAction(get(game).id, action);
	};

	const selectAction = (action: AvailableActionDto) => {
		if ($isSpectator) {
			return;
		}

		const workflow = fromAction($currentPlayer!.id, $game, action, store);
		startWorkflow(workflow);
		closeMenu();
	};

	$: useVerticalLayout = isMobile && $availableCommands.length > 1;
	$: isActivePlayer = $game?.activePlayer?.id === $currentPlayer?.id;
	$: showActionSelector = isActivePlayer && !$activeWorkflow;
	$: statusBarMessage = $isExecutingAction ? "Executing..." : $statusMessage;
	$: isIdle = !$isExecutingAction;
</script>

<div
	class="flex items-center justify-center h-full gap-1 md:gap-3 py-1 px-2 md:py-2 md:px-4 bg-white shadow-sm shadow-gray-500"
	class:flex-col={useVerticalLayout}
>
	<div class="flex-initial text-xs md:text-sm text-center text-gray-900 gaia-font">{statusBarMessage}</div>
	{#if !$isSpectator}
		{#if isIdle}
			<div class="flex flex-shrink-0 gap-1 md:gap-3">
				{#each $availableCommands as cmd (`${cmd.nextState}§${cmd.text}`)}
					<ion-button
						size={isMobile ? "small" : "default"}
						color={cmd.isPrimary ? "primary" : undefined}
						on:click={() => handleCommand(cmd)}
					>
						<span class="gaia-font">{cmd.text}</span>
					</ion-button>
				{/each}
			</div>
		{/if}
		{#if showActionSelector}
			<div class="flex-shrink-0 ml-1 md:ml-3 relative">
				<ion-button size={isMobile ? "small" : "default"} color="primary" disabled={showMenu} on:click={openMenu}>
					<span class="gaia-font">Actions</span>
				</ion-button>
			</div>
		{/if}

		<ion-modal breakpoints={[0, 0.5]} initial-breakpoint={0.5} is-open={showMenu}>
			<ion-header translucent>
				<ion-toolbar>
					<ion-title class="gaia-font">Actions</ion-title>
					<ion-buttons slot="end">
						<ion-button on:click={closeMenu}>Close</ion-button>
					</ion-buttons>
				</ion-toolbar>
			</ion-header>
			<ion-content fullscreen>
				<ion-list>
					{#each $availableActions as action, index (action.type)}
						<ion-item on:click={() => selectAction(action)} lines={index < $availableActions.length - 1 ? "full" : "none"}>
							<ion-label class="gaia-font">
								{action.description}
							</ion-label>
						</ion-item>
					{/each}
				</ion-list>
			</ion-content>
		</ion-modal>
	{/if}
</div>

<style>
	.action-selector {
		min-width: 250px;
	}
</style>
