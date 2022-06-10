<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemAvatar from "$components/list/ListItemAvatar.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";
	import type { GameSubLogDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceColors, getRaceImage } from "$utils/race-utils";

	export let subLog: GameSubLogDto;
	export let isAnotherPlayer: boolean;

	const [playerColor, textColor] = getRaceColors(subLog.race);
</script>

<ListItem --background-color={playerColor} --color={textColor} noPadding>
	{#if isAnotherPlayer}
		<ListItemAvatar src={assetUrl(`Races/${getRaceImage(subLog.race)}`)} />
	{/if}
	<ListItemText text={subLog.message} size="xs" />
</ListItem>
