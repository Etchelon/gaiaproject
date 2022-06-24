<script lang="ts" context="module">
	import { RoundBoosterType } from "$dto/enums";

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
	import type { RoundBoosterDto, RoundBoosterTileDto } from "$dto/interfaces";
	import { assetUrl, interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { noop } from "lodash";
	import { getGamePageContext } from "../GamePage.context";
	import { deriveRoundBoosterInteractionState } from "../store/selectors";
	import ActionToken from "./ActionToken.svelte";
	import PlayerMarker from "./PlayerMarker.svelte";

	export let booster: RoundBoosterTileDto | RoundBoosterDto;
	export let withPlayerInfo = false;
	export let nonInteractive = false;

	let availableBooster = booster as RoundBoosterTileDto;
	let actualType = withPlayerInfo ? InteractiveElementType.OwnRoundBooster : InteractiveElementType.RoundBooster;
	const { store, activeWorkflow } = getGamePageContext();
	const interactionState = deriveRoundBoosterInteractionState(actualType)(booster.id)(store);
	$: ({ clickable, selected } = $interactionState);
	$: boosterClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(booster.id, actualType);
		  }
		: noop;
</script>

<div style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
	<img class="wh-full absolute top-0 left-0" src={assetUrl(`Boards/RoundBoosters/${roundBoosterNames.get(booster.id)}.png`)} alt="" />
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
	<div class={interactiveElementClass(clickable, selected)} on:click={boosterClicked} />
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
