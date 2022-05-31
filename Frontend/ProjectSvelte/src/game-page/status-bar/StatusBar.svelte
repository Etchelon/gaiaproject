<script lang="ts">
	import Button from "$components/Button.svelte";
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";
	import { noop, random } from "lodash";
	import { getGamePageContext } from "../GamePage.context";

	export let isMobile: boolean;
	export let playerId: string | null;
	export let isSpectator: boolean;

	const { store } = getGamePageContext();

	$: game = $store.game;

	const isExecutingAction = random(true) > 0.95;
	const statusMessage = "TODO: from the store";
	const statusBarMessage = isExecutingAction ? "Executing..." : statusMessage;
	const isIdle = random(true) > 0.05;
	const activeWorkflow = random(true) > 0.5;
	const commands: any[] = [
		{
			text: "Build",
			isPrimary: true,
		},
		{
			text: "Form a Federation",
		},
	];
	const availableActions: any[] = [
		{
			type: 0,
			description: "Build",
		},
		{
			type: 1,
			description: "Form a Federation",
		},
	];

	const handleCommand = noop;

	let showMenu = false;
	const openMenu = () => {
		showMenu = true;
	};
	const closeMenu = () => {
		showMenu = false;
	};
	const selectAction = (action: any) => {};

	$: useVerticalLayout = isMobile && commands.length > 1;
	$: isActivePlayer = game.activePlayer?.id === playerId;
	$: showActionSelector = true; //isActivePlayer && !activeWorkflow;
</script>

<svelte:window on:click={closeMenu} />
<div
	class="flex items-center justify-center h-full gap-1 md:gap-3 py-1 px-2 md:py-2 md:px-4 bg-white shadow-sm shadow-gray-500"
	class:flex-col={useVerticalLayout}
>
	<div class="flex-initial text-xs md:text-sm text-center gaia-font">{statusBarMessage}</div>
	{#if !isSpectator}
		{#if isIdle}
			<div class="flex flex-shrink-0 gap-1 md:gap-3">
				{#each commands as cmd (`${cmd.nextState}§${cmd.text}`)}
					<Button
						size={isMobile ? "small" : "normal"}
						variant={cmd.isPrimary ? "primary" : "default"}
						on:click={() => handleCommand(cmd)}
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
						{#each availableActions as action (action.type)}
							<ListItem on:click={selectAction}>
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
