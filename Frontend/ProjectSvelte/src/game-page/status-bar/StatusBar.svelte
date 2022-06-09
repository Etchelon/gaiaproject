<script lang="ts">
	import Button from "$components/Button.svelte";
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";
	import type { AvailableActionDto } from "$dto/interfaces";
	import { get } from "svelte/store";
	import { getGamePageContext } from "../GamePage.context";
	import type { Command } from "../workflows/types";
	import { fromAction } from "../workflows/utils";

	export let isMobile: boolean;

	const { store, activeWorkflow, startWorkflow } = getGamePageContext();
	const { availableActions, availableCommands, currentPlayer, game, isExecutingAction, isSpectator, statusMessage } = store;

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

	let showMenu = false;
	const openMenu = () => {
		showMenu = true;
	};
	const closeMenu = () => {
		showMenu = false;
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

<svelte:window on:click={closeMenu} />
<div
	class="flex items-center justify-center h-full gap-1 md:gap-3 py-1 px-2 md:py-2 md:px-4 bg-white shadow-sm shadow-gray-500"
	class:flex-col={useVerticalLayout}
>
	<div class="flex-initial text-xs md:text-sm text-center text-gray-900 gaia-font">{statusBarMessage}</div>
	{#if !$isSpectator}
		{#if isIdle}
			<div class="flex flex-shrink-0 gap-1 md:gap-3">
				{#each $availableCommands as cmd (`${cmd.nextState}§${cmd.text}`)}
					<Button
						size={isMobile ? "small" : "normal"}
						variant={cmd.isPrimary ? "primary" : "default"}
						on:clicked={() => handleCommand(cmd)}
					>
						<span class="gaia-font">{cmd.text}</span>
					</Button>
				{/each}
			</div>
		{/if}
		{#if showActionSelector}
			<div class="flex-shrink-0 ml-1 md:ml-3 relative">
				<Button size={isMobile ? "small" : "normal"} variant="primary" on:clicked={openMenu}>
					<span class="gaia-font">Actions</span>
				</Button>
				{#if showMenu}
					<div class="action-selector absolute top-0 right-0 p-2 border-2 rounded-lg border-gray-300 bg-white shadow-xl">
						{#each $availableActions as action (action.type)}
							<ListItem on:click={() => selectAction(action)}>
								<ListItemText text={action.description} size="sm" />
							</ListItem>
						{/each}
					</div>
				{/if}
			</div>
		{/if}
	{/if}
</div>

<style>
	.action-selector {
		min-width: 250px;
	}
</style>
