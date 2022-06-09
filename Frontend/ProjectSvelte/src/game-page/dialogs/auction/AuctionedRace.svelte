<script lang="ts">
	import type { Race } from "$dto/enums";
	import type { AuctionDto } from "$dto/interfaces";
	import { getContrastColor } from "$utils/colors";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceColor, getRaceImage, getRaceName } from "$utils/race-utils";

	export let auction: AuctionDto;
	export let selected: boolean;
	export let onSelected: (r: Race) => void;

	$: race = auction.race;
	$: raceName = getRaceName(race);
	$: background = getRaceColor(race);
	$: color = getContrastColor(background);
	$: username = auction.playerUsername;
</script>

<div class="flex flex-col items-center" on:click={() => onSelected(race)}>
	<h6 class="gaia-font">
		{raceName}
	</h6>
	<div class="p-1 relative" style={`background-color: ${background}; color: ${color};`}>
		<ion-avatar>
			<img src={assetUrl(`Races/${getRaceImage(race)}`)} alt={raceName} />
		</ion-avatar>
		{#if selected}
			<div class="absolute -bottom-1 right-0">
				<ion-checkbox checked={true} />
			</div>
		{/if}
	</div>
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
