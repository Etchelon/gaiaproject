<script lang="ts">
	import type { Race } from "$dto/enums";
	import type { AuctionDto } from "$dto/interfaces";
	import { getRaceName } from "$utils/race-utils";
	import SelectableRaceAvatar from "../select-race/SelectableRaceAvatar.svelte";

	export let auction: AuctionDto;
	export let selected: boolean;
	export let onSelected: (r: Race) => void;

	$: race = auction.race;
	$: raceName = getRaceName(race);
	$: username = auction.playerUsername;
</script>

<div class="flex flex-col items-center">
	<h6 class="gaia-font">
		{raceName}
	</h6>
	<SelectableRaceAvatar {race} {selected} {onSelected} />
	{#if username}
		<div class="flex flex-col items-center mt-1 gaia-font">
			<p>
				{username}
			</p>
			<p>
				{`${auction.points} VP`}
			</p>
		</div>
	{/if}
</div>
