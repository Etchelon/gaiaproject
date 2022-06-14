<script lang="ts" context="module">
	import { Conversion } from "$dto/enums";
	import type { ResourcesDto } from "$dto/interfaces";
	import { cloneDeep } from "lodash";

	const MAX_ORES_KNOWLEDGE = 15;
	const MAX_CREDITS = 30;

	const sizeAndPosition = (width: number, height: number, top: number, left: number) => `
		width: ${width}%;
		height: ${height}%;
		top: ${top}%;
		left: ${left}%;
	`;

	const ores = (resources: ResourcesDto) => resources.ores;
	const credits = (resources: ResourcesDto) => resources.credits;
	const knowledge = (resources: ResourcesDto) => resources.knowledge;
	const qic = (resources: ResourcesDto) => resources.qic;

	interface PlayerWithConversions {
		resources: ResourcesDto;
		remainingPower: number;
		conversions: Conversion[];
	}

	interface ActionType {
		type: Conversion | "reset";
		data?: any;
	}

	const reducer = (state: PlayerWithConversions, action: ActionType): PlayerWithConversions => {
		const newState = cloneDeep(state);
		const resources = newState.resources;

		const spendPower = (amount: number) => {
			newState.remainingPower -= amount;
		};

		switch (action.type) {
			case "reset":
				return {
					resources: cloneDeep(action.data.resources as ResourcesDto),
					remainingPower: action.data.remainingPower,
					conversions: [],
				};
			case Conversion.PowerToQic:
				resources.qic += 1;
				spendPower(4);
				break;
			case Conversion.PowerToOre:
				resources.ores += 1;
				spendPower(3);
				break;
			case Conversion.PowerToKnowledge:
				resources.knowledge += 1;
				spendPower(4);
				break;
			case Conversion.PowerToCredit:
				spendPower(1);
				resources.credits += 1;
				break;
			default:
				throw new Error(`Action ${action} not implemented.`);
		}
		newState.conversions.push(action.type);
		return newState;
	};
</script>

<script lang="ts">
	import ClickableRectangle from "$components/ClickableRectangle.svelte";
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { isEmpty } from "lodash";
	import { get, writable } from "svelte/store";
	import ResourceToken from "../../game-board/ResourceToken.svelte";
	import { getGamePageContext } from "../../GamePage.context";
	import type { TerransDecideIncomeWorkflow } from "../../workflows/rounds-phase/terrans-decide-income.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";

	export let gameId: string;
	export let currentPlayer: PlayerInGameDto;

	let isPerformingConversion = false;
	const { store, activeWorkflow } = getGamePageContext();
	const tdiWorkflow = get(activeWorkflow) as TerransDecideIncomeWorkflow;
	const powerToConvert = tdiWorkflow?.powerToConvert ?? 0;
	const conversionsState = writable<PlayerWithConversions>({
		resources: cloneDeep(currentPlayer.state.resources),
		remainingPower: powerToConvert,
		conversions: [],
	});
	const dispatch = (action: ActionType) => {
		conversionsState.update(state => reducer(state, action));
	};

	const reset = () => {
		dispatch({ type: "reset", data: { resources: currentPlayer.state.resources, remainingPower: powerToConvert } });
	};

	const closeDialog = () => {
		$activeWorkflow?.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};

	const convert = () => {
		const action = $activeWorkflow!.handleCommand({
			nextState: CommonWorkflowStates.PERFORM_ACTION,
			data: $conversionsState.conversions,
		})!;
		store.executePlayerAction(gameId, action);
		isPerformingConversion = true;
	};

	$: resources = $conversionsState.resources;
	$: remainingPower = $conversionsState.remainingPower;
</script>

<ion-header>
	<ion-toolbar>
		<ion-title class="gaia-font">Convert power from Bowl 2</ion-title>
	</ion-toolbar>
</ion-header>
<ion-content fullscreen scroll-y={false}>
	<div class="grid grid-cols-2 gap-2 p-1 md:p-3">
		<div class="col-span-2 md:col-span-1">
			<div class="relative">
				<img class="w-full object-contain" src={assetUrl("TerransConversions.png")} alt="" />
				<ClickableRectangle
					style={sizeAndPosition(16, 16, 11, 60)}
					active={remainingPower >= 4}
					on:click={() => dispatch({ type: Conversion.PowerToQic })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 16, 34, 60)}
					active={remainingPower >= 3}
					on:click={() => dispatch({ type: Conversion.PowerToOre })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 16, 55, 60)}
					active={remainingPower >= 4}
					on:click={() => dispatch({ type: Conversion.PowerToKnowledge })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 16, 76, 60)}
					active={remainingPower >= 1}
					on:click={() => dispatch({ type: Conversion.PowerToCredit })}
				/>
			</div>
		</div>
		<div class="col-span-2 md:col-span-1">
			<div class="flex flex-col items-stretch h-full">
				<div class="player-data">
					<h6 class="gaia-font text-center">Remaining Power</h6>
					<div class="row mb-1">
						<div class="resource-indicator">
							<ResourceToken type="Power" />
							<span class="gaia-font">{remainingPower}</span>
						</div>
					</div>
					<h6 class="gaia-font text-center">Resources</h6>
					<div class="row">
						<div class="resource-indicator" class:maxed-out={ores(resources) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Ores" />
							<span class="gaia-font">{ores(resources)}</span>
						</div>
						<div class="resource-indicator" class:maxed-out={credits(resources) === MAX_CREDITS}>
							<ResourceToken type="Credits" />
							<span class="gaia-font">{credits(resources)}</span>
						</div>
						<div class="resource-indicator" class:maxed-out={knowledge(resources) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Knowledge" />
							<span class="gaia-font">{knowledge(resources)}</span>
						</div>
						<div class="resource-indicator">
							<ResourceToken type="Qic" />
							<span class="gaia-font">{qic(resources)}</span>
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
			<ion-button size="small" on:click={closeDialog}>
				<span class="gaia-font">Close</span>
			</ion-button>
			<ion-button
				size="small"
				color="primary"
				disabled={isPerformingConversion || isEmpty($conversionsState.conversions)}
				on:click={convert}
			>
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
