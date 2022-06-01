<script lang="ts">
	import type { FederationTokenStackDto, RoundBoosterTileDto, ScoringTrackDto } from "$dto/interfaces";
	import { onMount } from "svelte";
	import RoundBooster from "../RoundBooster.svelte";
	import FederationTokenStack from "./FederationTokenStack.svelte";
	import ScoringTrack from "./scoring-track/ScoringTrack.svelte";

	export let board: ScoringTrackDto;
	export let roundBoosters: RoundBoosterTileDto[];
	export let federationTokens: FederationTokenStackDto[];
	export let isMobile = false;

	let container: HTMLDivElement;
	let width: number;

	const getContainerWidth = () => {
		width = container.clientWidth;
	};
	onMount(getContainerWidth);
</script>

<svelte:window on:resize={getContainerWidth} />

<div class="scoring-board wh-full">
	<div class="lg:container lg:mx-auto">
		<div class="grid grid-cols-2 gap-2 md:gap-4">
			<div class="col-span-2 md:col-span-1" bind:this={container}>
				<ScoringTrack {board} {width} />
			</div>
			<div class="col-span-2 md:col-span-1 grid grid-rows-2 gap-2 md:gap-4">
				<div class="round-boosters wh-full">
					{#each roundBoosters as booster (booster.id)}
						<div class="booster">
							<RoundBooster {booster} />
						</div>
					{/each}
				</div>
				<div class="federation-tokens wh-full">
					{#each federationTokens.filter(stack => stack.remaining > 0) as stack (stack.type)}
						<div class="stack">
							<FederationTokenStack {stack} />
						</div>
					{/each}
				</div>
			</div>
		</div>
	</div>
</div>

<style>
	.scoring-board {
		align-self: flex-start;
		overflow: hidden;
	}

	.round-boosters {
		display: flex;
		flex-wrap: wrap;
		align-items: flex-start;
	}

	.round-boosters > .booster {
		width: calc((100% - 4 * (8px)) / 5);
		max-width: 100px;
		margin: 0 8px 8px 0;
	}

	.round-boosters > .booster:last-child {
		margin-right: 0;
	}

	.federation-tokens {
		display: flex;
		flex-wrap: wrap;
		align-items: flex-start;
	}

	.federation-tokens > .stack {
		width: 6%;
		min-width: 75px;
		max-width: 150px;
		margin: 0 12px 12px 0;
	}
</style>
