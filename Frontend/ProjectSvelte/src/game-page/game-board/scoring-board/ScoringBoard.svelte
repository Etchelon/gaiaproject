<script lang="ts">
	import type { FederationTokenStackDto, RoundBoosterTileDto, ScoringTrackDto } from "$dto/interfaces";
	import RoundBooster from "../RoundBooster.svelte";
	import FederationTokenStack from "./FederationTokenStack.svelte";
	import ScoringTrack from "./scoring-track/ScoringTrack.svelte";

	export let board: ScoringTrackDto;
	export let roundBoosters: RoundBoosterTileDto[];
	export let federationTokens: FederationTokenStackDto[];
	export let isMobile = false;
</script>

<div class="scoring-board fill-parent">
	<div class="container">
		<div class="grid">
			<div class="grid">
				<ScoringTrack {board} />
			</div>
			<div class="grid">
				<div class="round-boosters fill-parent">
					{#each roundBoosters as booster (booster.id)}
						<div class="booster">
							<RoundBooster {booster} />
						</div>
					{/each}
				</div>
			</div>
			<div class="grid">
				<div class="federation-tokens fill-parent">
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
