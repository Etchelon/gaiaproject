<script lang="ts" context="module">
	const BOOSTER_TO_BOARD_RATIO = 16;
</script>

<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { isEmpty, isNil } from "lodash";
	import FederationToken from "../FederationToken.svelte";
	import RoundBooster from "../RoundBooster.svelte";
	import PlayerBoard from "./PlayerBoard.svelte";
	import PlayerTechnologyTile from "./PlayerTechnologyTile.svelte";

	export let player: PlayerInGameDto;
	export let framed = true;
</script>

{#if !isNil(player.state)}
	<div class={framed ? "p-1 sm:p-2 md:p-2 border-1 md:border-2 border-gray-500 rounded-md" : ""}>
		<div class="flex items-stretch">
			<div class="flex-initial" style:width={`calc(100% * ${BOOSTER_TO_BOARD_RATIO - 1} / ${BOOSTER_TO_BOARD_RATIO})`}>
				<PlayerBoard {player} />
			</div>
			<div class="flex-shrink-0 w-2 sm:w-3 md:w-4" />
			<div class="flex flex-col flex-shrink-0" style:width={`calc(100% / ${BOOSTER_TO_BOARD_RATIO})`}>
				{#if player.state.roundBooster}
					<div class="w-full mb-2 md:mb-4">
						<RoundBooster booster={player.state.roundBooster} withPlayerInfo={true} />
					</div>
				{/if}
				<div class="w-full flex flex-wrap justify-center">
					{#each player.state.federationTokens as token, index (index)}
						<div class="w-3/4 mb-3 last:mb-0">
							<FederationToken type={token.id} playerId={player.id} used={token.usedForTechOrAdvancedTile} />
						</div>
					{/each}
				</div>
			</div>
		</div>
		{#if !isEmpty(player.state.technologyTiles)}
			<div class="flex flex-wrap mt-1 md:mt-3">
				{#each player.state.technologyTiles as tile (tile.id)}
					<div class="tech-tile-wrapper mr-1 md:mr-3">
						<PlayerTechnologyTile {tile} playerId={player.id} />
					</div>
				{/each}
			</div>
		{/if}
	</div>
{/if}

<style>
	.tech-tile-wrapper {
		width: 10%;
	}
</style>
