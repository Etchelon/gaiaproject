<script lang="ts">
	import type { Race } from "$dto/enums";
	import { getContrastColor } from "$utils/colors";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceColor, getRaceImage, getRaceName } from "$utils/race-utils";

	export let race: Race;
	export let selected: boolean;
	export let onSelected: (r: Race) => void;

	$: background = getRaceColor(race);
	$: color = getContrastColor(background);
	$: imgUrl = assetUrl(`Races/${getRaceImage(race)}`);
	$: raceName = getRaceName(race);
</script>

<div class="p-1 relative cursor-pointer" style={`background-color: ${background}; color: ${color};`} on:click={() => onSelected(race)}>
	<ion-avatar>
		<img src={imgUrl} alt={raceName} />
	</ion-avatar>
	{#if selected}
		<div class="absolute -bottom-1 right-0">
			<ion-checkbox checked={true} disabled={true} />
		</div>
	{/if}
</div>
