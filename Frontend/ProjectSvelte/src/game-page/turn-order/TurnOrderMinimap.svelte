<script lang="ts">
	import { GamePhase } from "$dto/enums";
	import type { GameStateDto } from "$dto/interfaces";
	import { isAuctionOngoing, isLastRound } from "$utils/miscellanea";
	import { selectSortedActivePlayers, selectSortedPassedPlayers } from "$utils/selectors";
	import { some } from "lodash";
	import OngoingAuction from "./OngoingAuction.svelte";
	import PlayerAvatar from "./PlayerAvatar.svelte";

	export let game: GameStateDto;
	export let direction: "vertical" | "horizontal";

	const isAuctioning = isAuctionOngoing(game);
	const auctionedRaces = game.auctionState?.auctionedRaces ?? [];
	const isLastRound_ = isLastRound(game);
	const activePlayers = selectSortedActivePlayers(game);
	const passedPlayers = selectSortedPassedPlayers(game);
</script>

{#if isAuctioning}
	<div class="p-2 bg-black border-2 border-gray-400 border-solid rounded-lg">
		<div class="gaia-font">Auction</div>
		<div class={direction === "vertical" ? "flex flex-col justify-center items-center" : "flex"}>
			{#each auctionedRaces as auction, index (index)}
				<div class="w-full">
					<OngoingAuction
						race={auction.race}
						username={auction.playerUsername ?? "No bid"}
						orderLabel={`${auction.order + 1}°`}
					/>
				</div>
			{/each}
		</div>
	</div>
{:else}
	<div class="p-2 bg-black border-2 border-gray-400 border-solid rounded-lg">
		<div>
			<p class="m-0 text-white gaia-font">
				Round {game.currentRound}
			</p>
			<div class={direction === "vertical" ? "flex flex-col justify-center items-center" : "flex"}>
				{#each activePlayers as p (p.id)}
					<div class={direction === "vertical" ? "mt-1" : "ml-2"}>
						<PlayerAvatar race={p.raceId} username={p.username} orderLabel={`${p.state?.currentRoundTurnOrder ?? 0}°`} />
					</div>
				{/each}
			</div>
		</div>
		{#if game.currentPhase !== GamePhase.Setup && !isLastRound_ && some(passedPlayers)}
			<div class="ml-2">
				<p class="m-0 text-white gaia-font">
					Round {game.currentRound + 1}
				</p>
				<div class={direction === "vertical" ? "flex flex-col justify-center items-center" : "flex"}>
					{#each passedPlayers as pp (pp.id)}
						<div class={direction === "vertical" ? "mt-1" : "ml-2"}>
							<PlayerAvatar race={pp.raceId} username={pp.username} orderLabel={`${pp.state.nextRoundTurnOrder}°`} />
						</div>
					{/each}
				</div>
			</div>
		{/if}
	</div>
{/if}
