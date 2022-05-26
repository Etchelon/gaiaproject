<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemAvatar from "$components/list/ListItemAvatar.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";
	import type { GameSubLogDto } from "$dto/interfaces";
	import { getContrastColor } from "$utils/colors";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceColor, getRaceImage } from "$utils/race-utils";

	export let subLog: GameSubLogDto;
	export let isAnotherPlayer: boolean;

	const playerColor = getRaceColor(subLog.race);
	const textColor = getContrastColor(playerColor);
</script>

<ListItem --background-color={playerColor} --color={textColor} noPadding>
	{#if isAnotherPlayer}
		<ListItemAvatar src={assetUrl(`Races/${getRaceImage(subLog.race)}`)} />
	{/if}
	<ListItemText text={subLog.message} size="xs" />
</ListItem>
