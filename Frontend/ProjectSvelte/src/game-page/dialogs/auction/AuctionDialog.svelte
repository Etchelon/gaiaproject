<script lang="ts" context="module">
	import type { CurrentAuction } from "../../workflows/setup-phase/bid-for-race.workflow";

	const boardVersion = "rework";

	interface ActionType {
		type: "select" | "increase" | "decrease";
		selection?: CurrentAuction;
		variation?: number;
		minimumBid?: number;
	}

	const reducer = (state: CurrentAuction, action: ActionType): CurrentAuction => {
		switch (action.type) {
			case "select":
				return { ...action.selection! };
			case "increase":
			case "decrease":
				return {
					race: state.race!,
					bid: Math.max(action.minimumBid!, state.bid! + (action.type === "increase" ? 1 : -1) * action.variation!),
				};
			default:
				throw new Error(`Action of type ${action.type} not handled.`);
		}
	};
</script>

<script lang="ts">
	import Button from "$components/Button.svelte";
	import type { Race } from "$dto/enums";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceBoard, getRaceName } from "$utils/race-utils";
	import { chain, isNil, isNull } from "lodash";
	import { writable } from "svelte/store";
	import { getGamePageContext } from "../../GamePage.context";
	import type { BidForRaceWorkflow } from "../../workflows/setup-phase/bid-for-race.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";
	import AuctionedRace from "./AuctionedRace.svelte";

	export let gameId: string;

	const { store, activeWorkflow } = getGamePageContext();
	const currentAuction = writable<{ race: Race | null; bid: number | null }>({ race: null, bid: null });
	const dispatch = (action: ActionType) => {
		currentAuction.update(state => reducer(state, action));
	};
	let isBidding = false;

	const bid = (type: "increase" | "decrease", amount: number) => {
		dispatch({ type, variation: amount, minimumBid: minimumBidForSelectedRace });
	};
	const bidMore = (amount: number) => bid("increase", amount);
	const bidLess = (amount: number) => bid("decrease", amount);
	const select = (race: Race) => {
		const auctionForSelectedRace = auctions.find(o => o.race === race)!;
		const minimumBid = auctionForSelectedRace.points !== null ? auctionForSelectedRace.points + 1 : 0;
		dispatch({ type: "select", selection: { race, bid: minimumBid } });
	};

	const closeDialog = () => {
		$activeWorkflow?.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};
	const doBid = () => {
		const action = $activeWorkflow?.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: currentAuction })!;
		isBidding = true;
		store.executePlayerAction(gameId, action);
	};

	$: auctionState = ($activeWorkflow as BidForRaceWorkflow)?.auctionState;
	$: auctions = auctionState?.auctionedRaces ?? [];
	$: selectedRace = $currentAuction.race;
	$: currentBid = $currentAuction.bid;
	$: ongoingAuctionForSelectedRace = auctions.find(o => o.race === selectedRace);
	$: isOngoingAuction = ongoingAuctionForSelectedRace?.points !== null;
	$: minimumBidForSelectedRace =
		ongoingAuctionForSelectedRace && ongoingAuctionForSelectedRace !== null ? ongoingAuctionForSelectedRace.points! + 1 : 0;
	$: isLastRace =
		selectedRace !== null &&
		chain(auctions)
			.filter(o => o.race !== selectedRace)
			.every(o => o.points !== null)
			.value();
	$: canBid0Points = isLastRace || !isOngoingAuction;
</script>

<div class="overflow-x-hidden overflow-y-auto p-1 md:p-2">
	<h6 class="header pt-1 pb-2 gaia-font text-center">Bid for a race</h6>
	<div class="race-list flex w-hull overflow-x-auto overflow-y-hidden">
		{#each auctions as auction (auction.race)}
			<div class="contents">
				<AuctionedRace {auction} selected={auction.race === selectedRace} onSelected={select} />
				<div class="mr-2" />
			</div>
		{/each}
	</div>
	<div class="w-100 my-2">
		{#if isNull(selectedRace)}
			<h5>Select a race to view its board</h5>
		{:else}
			<img class="w-100" src={assetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(selectedRace)}`)} alt="" />
		{/if}
	</div>
	{#if selectedRace}
		<div class="mb-2 flex justify-center">
			{#if canBid0Points}
				<h6 class="gaia-font">
					Play {getRaceName(selectedRace)} for 0 VP
				</h6>
			{:else}
				<Button size="small" disabled={currentBid === minimumBidForSelectedRace} on:clicked={() => bidLess(1)}>
					<span class="gaia-font">-1</span>
				</Button>
				<h6 class="mx-2 gaia-font">
					Bid {currentBid} VP for {getRaceName(selectedRace)}
				</h6>
				<Button size="small" on:clicked={() => bidMore(1)}>
					<span class="gaia-font">+1</span>
				</Button>
			{/if}
		</div>
	{/if}
	<div class="mt-auto flex justify-end">
		<Button size="small" on:clicked={closeDialog}>
			<span class="gaia-font">Close</span>
		</Button>
		<Button variant="primary" disabled={isNil(selectedRace) || isBidding} on:clicked={doBid}>
			<span class="gaia-font">Confirm</span>
		</Button>
	</div>
</div>

<style>
	.header {
		min-width: 300px;
	}

	.race-list {
		scrollbar-width: none;
	}

	.race-list::-webkit-scrollbar {
		display: none;
	}
</style>
