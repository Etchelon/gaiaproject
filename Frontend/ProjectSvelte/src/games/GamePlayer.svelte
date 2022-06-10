<script lang="ts">
	import type { PlayerInfoDto } from "$dto/interfaces";
	import { assetUrl, playerInitials } from "$utils/miscellanea";
	import { getRaceColors, getRaceImage } from "$utils/race-utils";
	import { isNil } from "lodash";

	export let player: PlayerInfoDto;

	let chipColorsStyle: string;
	$: playerImg = !isNil(player.raceId)
		? assetUrl(`Races/${getRaceImage(player.raceId)}`)
		: player.avatarUrl ?? `https://ui-avatars.com/api/?name=${player.username}`;
	$: playerLabel = `${!isNil(player.placement) ? `${player.placement}° - ` : ""}${player.username}`;
	$: {
		if (!isNil(player.raceId)) {
			const [background, color] = getRaceColors(player.raceId);
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
