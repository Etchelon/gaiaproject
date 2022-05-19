<script lang="ts" context="module">
	import { RoundBoosterType } from "$dto/enums";
	import type { RoundBoosterDto, RoundBoosterTileDto } from "$dto/interfaces";
	import { assetUrl, interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { random } from "lodash";

	const WIDTH_TO_HEIGHT_RATIO = 0.3423;
	const roundBoosterNames = new Map<RoundBoosterType, string>([
		[RoundBoosterType.GainOreGainKnowledge, "BOOknw"],
		[RoundBoosterType.GainPowerTokensGainOre, "BOOpwt"],
		[RoundBoosterType.GainCreditsGainQic, "BOOqic"],
		[RoundBoosterType.TerraformActionGainCredits, "BOOter"],
		[RoundBoosterType.BoostRangeGainPower, "BOOnav"],
		[RoundBoosterType.PassPointsPerMineGainOre, "BOOmin"],
		[RoundBoosterType.PassPointsPerTradingStationsGainOre, "BOOtrs"],
		[RoundBoosterType.PassPointsPerResearchLabsGainKnowledge, "BOOlab"],
		[RoundBoosterType.PassPointsPerBigBuildingsGainPower, "BOOpia"],
		[RoundBoosterType.PassPointsPerGaiaPlanetsGainCredits, "BOOgai"],
	]);
</script>

<script lang="ts">
	import ActionToken from "./ActionToken.svelte";
	import PlayerMarker from "./PlayerMarker.svelte";

	export let booster: RoundBoosterTileDto | RoundBoosterDto;
	export let withPlayerInfo = false;
	export let nonInteractive = false;

	let availableBooster = booster as RoundBoosterTileDto;
	let actualType = withPlayerInfo ? InteractiveElementType.OwnRoundBooster : InteractiveElementType.RoundBooster;
	let clickable = random(true) > 0.5 && !nonInteractive;
	let selected = random(true) > 0.75 && !nonInteractive;
</script>

<div style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
	<img class="fill-parent-abs" src={assetUrl(`Boards/RoundBoosters/${roundBoosterNames.get(booster.id)}.png`)} alt="" />
	{#if booster.used}
		<div class="action-token">
			<ActionToken />
		</div>
	{/if}
	{#if availableBooster?.isTaken}
		<div class="player-marker">
			<PlayerMarker race={availableBooster.player.raceId ?? 0} />
		</div>
	{/if}
	<div class={interactiveElementClass(clickable, selected)} on:click />
</div>

<style>
	.action-token {
		position: absolute;
		width: 70%;
		top: 17%;
		left: 15%;
	}

	.player-marker {
		position: absolute;
		width: 40%;
		bottom: 3%;
		right: 10%;
	}
</style>
