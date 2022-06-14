<script lang="ts" context="module">
	import { Conversion } from "$dto/enums";
	import type { ResourcesDto } from "$dto/interfaces";
	import { cloneDeep } from "lodash";

	const sizeAndPosition = (width: number, height: number, top: number, left: number) => `
		width: ${width}px;
		height: ${height}px;
		top: ${top}px;
		left: ${left}px;
	`;

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
	import { writable } from "svelte/store";
	import { getGamePageContext } from "../../GamePage.context";
	import type { TerransDecideIncomeWorkflow } from "../../workflows/rounds-phase/terrans-decide-income.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";

	export let gameId: string;
	export let currentPlayer: PlayerInGameDto;

	let isPerformingConversion = false;
	const { store, activeWorkflow } = getGamePageContext();
	const tdiWorkflow = $activeWorkflow as TerransDecideIncomeWorkflow;
	const powerToConvert = tdiWorkflow?.powerToConvert ?? 0;
	const conversionsState = writable<PlayerWithConversions>({
		resources: cloneDeep(currentPlayer.state.resources),
		remainingPower: powerToConvert,
		conversions: [],
	});
	const dispatch = (action: ActionType) => {
		conversionsState.update(state => reducer(state, action));
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
<ion-content scroll-y={false}>
	<div class="grid gap-2">
		<div class="col-span-12 md:col-span-6">
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
		<div class="col-span-12 md:col-span-6">
			<div class="flex flex-col items-stretch h-full">
				<div class={`${styles.playerData} ${classes.playerData}`}>
					<Typography variant="h6" class="gaia-font text-center">Remaining Power</Typography>
					<div class={`${styles.row} ${classes.statusRow}`}>
						<div class={styles.resourceIndicator}>
							<ResourceToken type="Power" />
							<span class="gaia-font">{remainingPower}</span>
						</div>
					</div>
					<Typography variant="h6" class="gaia-font text-center">Resources</Typography>
					<div class={`${styles.row} ${classes.statusRow}`}>
						<div class={styles.resourceIndicator}>
							<ResourceToken type="Ores" />
							<span class="gaia-font">{ores(resources)}</span>
						</div>
						<div class={styles.resourceIndicator}>
							<ResourceToken type="Credits" />
							<span class="gaia-font">{credits(resources)}</span>
						</div>
						<div class={styles.resourceIndicator}>
							<ResourceToken type="Knowledge" />
							<span class="gaia-font">{knowledge(resources)}</span>
						</div>
						<div class={styles.resourceIndicator}>
							<ResourceToken type="Qic" />
							<span class="gaia-font">{qic(resources)}</span>
						</div>
					</div>
				</div>
				<div class={classes.commands}>
					<Button
						variant="contained"
						class="command"
						onClick={() =>
							dispatch({ type: "reset", data: { resources: currentPlayer.state.resources, remainingPower: powerToConvert } })}
					>
						<span class="gaia-font">Reset</span>
					</Button>
					<Button variant="contained" class="command" onClick={closeDialog}>
						<span class="gaia-font">Close</span>
					</Button>
					<Button
						variant="contained"
						color="primary"
						class="command"
						disabled={isPerformingConversion || isEmpty(conversionsState.conversions)}
						onClick={convert}
					>
						<span class="gaia-font">Confirm</span>
					</Button>
				</div>
			</div>
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
