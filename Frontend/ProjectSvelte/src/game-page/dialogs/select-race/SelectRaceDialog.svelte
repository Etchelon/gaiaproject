<script lang="ts">
	import type { Race } from "$dto/enums";
	import { isNil } from "lodash";
	import { getGamePageContext } from "../../GamePage.context";
	import type { SelectRaceWorkflow } from "../../workflows/setup-phase/select-race.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";
	import RaceBoard from "../RaceBoard.svelte";
	import SelectableRaceAvatar from "./SelectableRaceAvatar.svelte";

	export let gameId: string;

	const { store, activeWorkflow } = getGamePageContext();
	let isSelecting = false;
	let selectedRace: Race | null;

	const closeDialog = () => {
		$activeWorkflow?.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};
	const confirmSelection = () => {
		const action = $activeWorkflow?.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: selectedRace })!;
		isSelecting = true;
		store.executePlayerAction(gameId, action);
	};

	$: availableRaces = ($activeWorkflow as SelectRaceWorkflow)?.availableRaces ?? [];
</script>

<ion-header>
	<ion-toolbar>
		<ion-title class="gaia-font">Select a race</ion-title>
	</ion-toolbar>
</ion-header>
<ion-content scroll-y={false}>
	<div class="p-1 md:p-2">
		<div class="race-list flex flex-wrap gap-1 md:gap-3 w-100">
			{#each availableRaces as race}
				<SelectableRaceAvatar
					{race}
					selected={race === selectedRace}
					onSelected={r => {
						selectedRace = r;
					}}
				/>
				<div class="mr-2" />
			{/each}
		</div>
		<div class="w-100 my-2">
			{#if isNil(selectedRace)}
				<h5>Select a race to view its board</h5>
			{:else}
				<RaceBoard race={selectedRace} />
			{/if}
		</div>
	</div>
</ion-content>
<ion-footer>
	<ion-toolbar>
		<ion-buttons slot="end">
			<ion-button size="small" on:click={closeDialog}>
				<span class="gaia-font">Close</span>
			</ion-button>
			<ion-button size="small" color="primary" disabled={isNil(selectedRace) || isSelecting} on:click={confirmSelection}>
				<span class="gaia-font">Confirm</span>
			</ion-button>
		</ion-buttons>
	</ion-toolbar>
</ion-footer>

<style>
	.race-list {
		scrollbar-width: none;
	}

	.race-list::-webkit-scrollbar {
		display: none;
	}
</style>
