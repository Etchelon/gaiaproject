<script lang="ts" context="module">
	import { BrainstoneLocation, Race, SortableIncomeType } from "$dto/enums";
	import type { PlayerStateDto, SortableIncomeDto } from "$dto/interfaces";
	import { cloneDeep } from "lodash";

	const brainstone = (playerState: PlayerStateDto) => playerState.resources.power.brainstone;
	const power1 = (playerState: PlayerStateDto) => playerState.resources.power.bowl1;
	const power2 = (playerState: PlayerStateDto) => playerState.resources.power.bowl2;
	const power3 = (playerState: PlayerStateDto) => playerState.resources.power.bowl3;
	const powerGaia = (playerState: PlayerStateDto) => playerState.resources.power.gaiaArea;

	const gainPower = (player: PlayerInGameDto, amount: number) => {
		const playerState = player.state;
		let brainstone_ = brainstone(playerState);
		const involvesBrainstone =
			player.raceId === Race.Taklons && (brainstone_ === BrainstoneLocation.Bowl1 || brainstone_ === BrainstoneLocation.Bowl2);

		if (involvesBrainstone) {
			if (brainstone_ === BrainstoneLocation.Bowl1) {
				amount -= 1;
				brainstone_ = BrainstoneLocation.Bowl2;
			}
		}
		if (amount === 0) {
			return;
		}

		let power1_ = power1(playerState);
		let power2_ = power2(playerState);
		let power3_ = power3(playerState);

		const setPowerValues = (p1: number, p2: number, p3: number, b: Nullable<BrainstoneLocation>) => {
			playerState.resources.power.bowl1 = p1;
			playerState.resources.power.bowl2 = p2;
			playerState.resources.power.bowl3 = p3;
			playerState.resources.power.brainstone = b;
		};

		if (power1_ > 0) {
			if (amount <= power1_) {
				power1_ -= amount;
				power2_ += amount;
				setPowerValues(power1_, power2_, power3_, brainstone_);
				return;
			} else {
				power2_ += power1_;
				amount -= power1_;
				power1_ = 0;
			}
		}
		if (amount === 0) {
			setPowerValues(power1_, power2_, power3_, brainstone_);
			return;
		}

		if (involvesBrainstone) {
			amount -= 1;
			brainstone_ = BrainstoneLocation.Bowl3;
		}
		if (amount === 0) {
			setPowerValues(power1_, power2_, power3_, brainstone_);
			return;
		}

		if (power2_ > 0) {
			if (amount <= power2_) {
				power2_ -= amount;
				power3_ += amount;
				setPowerValues(power1_, power2_, power3_, brainstone_);
				return;
			} else {
				power3_ += power2_;
				power2_ = 0;
			}
		}

		setPowerValues(power1_, power2_, power3_, brainstone_);
	};

	interface SortingState {
		player: PlayerInGameDto;
		allIncomes: SortableIncomeDto[];
		sortedIncomes: SortableIncomeDto[];
	}

	interface ActionType {
		type: "push" | "reset";
		data?: any;
	}

	const reducer = (state: SortingState, action: ActionType): SortingState => {
		const newState = cloneDeep(state);
		const player = newState.player;

		switch (action.type) {
			case "reset":
				return { player: cloneDeep(action.data.player), allIncomes: cloneDeep(action.data.allIncomes), sortedIncomes: [] };
			case "push":
				const income = action.data as SortableIncomeDto;
				newState.sortedIncomes.push(income);
				switch (income.type) {
					case SortableIncomeType.Power:
						gainPower(player, income.amount);
						break;
					case SortableIncomeType.PowerToken:
						let amountToGain = income.amount;
						const involvesBrainstone =
							player.raceId === Race.Taklons && brainstone(player.state) === BrainstoneLocation.Removed;
						if (involvesBrainstone) {
							amountToGain -= 1;
							player.state.resources.power.brainstone = BrainstoneLocation.Bowl1;
						}
						player.state.resources.power.bowl1 += amountToGain;
						break;
					default:
						throw new Error("Impossible.");
				}
				return newState;
			default:
				throw new Error(`Action ${action.type} not implemented.`);
		}
	};
</script>

<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import type { Nullable } from "$utils/miscellanea";
	import { isUndefined, size } from "lodash";
	import { get, writable } from "svelte/store";
	import ResourceToken from "../../game-board/ResourceToken.svelte";
	import { getGamePageContext } from "../../GamePage.context";
	import type { SortIncomesWorkflow } from "../../workflows/rounds-phase/sort-incomes.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";

	export let gameId: string;
	export let currentPlayer: PlayerInGameDto;

	let isExecuting = false;
	const { store, activeWorkflow: aw } = getGamePageContext();
	const activeWorkflow = get(aw);
	const sortIncomesWorkflow = activeWorkflow as Nullable<SortIncomesWorkflow>;
	const unsortedIncomes = sortIncomesWorkflow?.unsortedIncomes ?? [];
	const sortingState = writable<SortingState>({
		player: cloneDeep(currentPlayer),
		allIncomes: cloneDeep(unsortedIncomes),
		sortedIncomes: [],
	});
	const dispatch = (action: ActionType) => {
		sortingState.update(state => reducer(state, action));
	};

	const reset = () => {
		dispatch({ type: "reset", data: { player: currentPlayer, allIncomes: unsortedIncomes } });
	};

	const sort = () => {
		const action = activeWorkflow?.handleCommand({
			nextState: CommonWorkflowStates.PERFORM_ACTION,
			data: sortedIncomes.map(si => si.id),
		})!;
		store.executePlayerAction(gameId, action);
		isExecuting = true;
	};

	$: allIncomes = $sortingState.allIncomes;
	$: playerState = $sortingState.player.state;
	$: sortedIncomes = $sortingState.sortedIncomes;
</script>

<ion-header>
	<ion-toolbar>
		<ion-title class="gaia-font">Sort power incomes</ion-title>
	</ion-toolbar>
</ion-header>
<ion-content fullscreen>
	<div class="grid grid-cols-3 gap-2 p-1 md:p-3">
		<div class="col-span-3 md:col-span-1">
			<div class="h-full overflow-x-hidden overflow-y-auto">
				<h6 class="gaia-font text-center">Incomes to sort</h6>
				{#each allIncomes as income, index (index)}
					<ion-button
						class="w-full mt-1 py-1 px-2"
						disabled={!isUndefined(sortedIncomes.find(si => si.id === income.id))}
						onClick={() => dispatch({ type: "push", data: income })}
					>
						<span class="gaia-font">{income.description}</span>
					</ion-button>
				{/each}
			</div>
		</div>
		<div class="col-span-3 md:col-span-2">
			<div class="flex flex-col items-stretch h-full pt-1">
				<div class="player-data">
					<h6 class="gaia-font text-center">Power Bowls</h6>
					<div class="flex flex-wrap items-start justify-between mt-1">
						<div class="resource flex flex-col items-center">
							<p class="gaia-font">Bowl 1</p>
							<div class="resource-indicator">
								<ResourceToken type="Power" />
								<span class="gaia-font"
									>{`${power1(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span
								>
							</div>
						</div>
						<div class="resource flex flex-col items-center">
							<p class="gaia-font">Bowl 2</p>
							<div class="resource-indicator">
								<ResourceToken type="Power" />
								<span class="gaia-font"
									>{`${power2(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl2 ? " (B)" : ""}`}</span
								>
							</div>
						</div>
						<div class="resource flex flex-col items-center">
							<p class="gaia-font">Bowl 3</p>
							<div class="resource-indicator">
								<ResourceToken type="Power" />
								<span class="gaia-font"
									>{`${power3(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl3 ? " (B)" : ""}`}</span
								>
							</div>
						</div>
						<div class="resource flex flex-col items-center">
							<p class="gaia-font">Gaia</p>
							<div class="resource-indicator">
								<ResourceToken type="Power" />
								<span class="gaia-font"
									>{`${powerGaia(playerState)}${
										brainstone(playerState) === BrainstoneLocation.GaiaArea ? " (B)" : ""
									}`}</span
								>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</ion-content>
<ion-footer>
	<ion-toolbar>
		<ion-buttons slot="end">
			<ion-button size="small" color="warning" on:click={reset}>
				<span class="gaia-font">Reset</span>
			</ion-button>
			<ion-button size="small" color="primary" disabled={isExecuting || size(allIncomes) !== size(sortedIncomes)} on:click={sort}>
				<span class="gaia-font">Confirm</span>
			</ion-button>
		</ion-buttons>
	</ion-toolbar>
</ion-footer>

<style lang="scss">
	$spacingHalf: 4px;
	$spacing1: 8px;

	.player-data {
		padding: $spacingHalf $spacing1;
		background-color: white;
		color: black;

		.row {
			display: flex;
			align-items: center;
			justify-content: space-between;
			height: 30px;
		}

		.resource {
			min-width: 100px;
		}

		.resource-indicator {
			display: flex;
			align-items: center;

			> :global(img),
			> :global(.building) {
				width: 35px;
				margin-right: $spacingHalf;
			}

			> span {
				font-size: 0.75rem;
			}
		}

		.maxed-out > span {
			color: rgb(210, 19, 24);
		}
	}
</style>
