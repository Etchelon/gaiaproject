<script lang="ts">
	import type { PlayerInfoDto } from "$dto/interfaces";
	import { assetUrl, playerInitials } from "$utils/miscellanea";
	import { getRaceImage } from "$utils/race-utils";
	import { isNil } from "lodash";

	export let player: PlayerInfoDto;

	$: playerImg = !isNil(player.raceId) ? assetUrl(`Races/${getRaceImage(player.raceId)}`) : player.avatarUrl;
	$: playerLabel = `${!isNil(player.placement) ? `${player.placement}° - ` : ""}${player.username}`;
</script>

<ion-chip>
	<ion-avatar>
		<img src={playerImg} alt={playerInitials(player)} />
	</ion-avatar>
	<ion-label>
		<h4>{playerLabel}</h4>
	</ion-label>
</ion-chip>
