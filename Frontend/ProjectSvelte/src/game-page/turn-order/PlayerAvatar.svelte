<script lang="ts">
	import type { Race } from "$dto/enums";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceImage } from "$utils/race-utils";
	import { isNil } from "lodash";

	export let race: Race | null;
	export let username: string;
	export let orderLabel: string;

	const RACE_AVATAR_WIDTH = 50;
</script>

{#if !isNil(race)}
	<div class="relative">
		<div class="absolute bottom-0 left-0 text-white gaia-font">
			{orderLabel}
		</div>
		<img style:width={`${RACE_AVATAR_WIDTH}px`} src={assetUrl(`Races/${getRaceImage(race)}`)} alt="" />
	</div>
{:else}
	<div class="text-sm gaia-font">
		{username.substring(0, 3)}
	</div>
{/if}
