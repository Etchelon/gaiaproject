<script lang="ts">
	import type { Race } from "$dto/enums";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceImage } from "$utils/race-utils";

	export let race: Race;
	export let username: string;
	export let orderLabel: string;

	const RACE_AVATAR_WIDTH = 50;
</script>

<div class="flex items-center justify-start">
	<div class="relative">
		<div class="absolute bottom-0 left-0 text-white gaia-font">
			{orderLabel}
		</div>
		<img style:width={`${RACE_AVATAR_WIDTH}px`} src={assetUrl(`Races/${getRaceImage(race)}`)} alt="" />
	</div>
	<small class="text-sm gaia-font ml-2 overflow-x-hidden ellipsify">
		{username.substring(0, 3)}
	</small>
</div>
