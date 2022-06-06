<script lang="ts">
	import type { PlayerInfoDto } from "$dto/interfaces";
	import { getContrastColor } from "$utils/colors";
	import { assetUrl, playerInitials } from "$utils/miscellanea";
	import { getRaceColor, getRaceImage } from "$utils/race-utils";
	import { isNil } from "lodash";

	export let player: PlayerInfoDto;

	let chipColorsStyle: string;
	$: playerImg = !isNil(player.raceId)
		? assetUrl(`Races/${getRaceImage(player.raceId)}`)
		: player.avatarUrl ?? `https://ui-avatars.com/api/?name=${player.username}`;
	$: playerLabel = `${!isNil(player.placement) ? `${player.placement}° - ` : ""}${player.username}`;
	$: {
		if (!isNil(player.raceId)) {
			const background = getRaceColor(player.raceId);
			const color = getContrastColor(background);
			chipColorsStyle = `--color: ${color}; --background: ${background}`;
		}
	}
</script>

<ion-chip style={chipColorsStyle}>
	<ion-avatar>
		<img src={playerImg} alt={playerInitials(player)} />
	</ion-avatar>
	<ion-label>
		<h4 class="gaia-font">{playerLabel}</h4>
	</ion-label>
</ion-chip>

<style>
	ion-chip {
		height: 48px;
	}

	ion-chip ion-avatar {
		width: 36px;
		height: auto;
	}
</style>
