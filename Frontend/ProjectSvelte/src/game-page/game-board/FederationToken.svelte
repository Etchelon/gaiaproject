<script lang="ts" context="module">
	import { FederationTokenType } from "$dto/enums";

	const WIDTH_TO_HEIGHT_RATIO = 0.816;
	const federationTokenImages = new Map<FederationTokenType, string>([
		[FederationTokenType.Knowledge, "FEDknw"],
		[FederationTokenType.Credits, "FEDcre"],
		[FederationTokenType.Ores, "FEDore"],
		[FederationTokenType.PowerTokens, "FEDpwt"],
		[FederationTokenType.Qic, "FEDqic"],
		[FederationTokenType.Points, "FEDvps"],
		[FederationTokenType.Gleens, "FEDgle"],
	]);
</script>

<script lang="ts">
	import { assetUrl, interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { isUndefined, random } from "lodash";

	export let type: FederationTokenType;
	export let playerId: string | undefined = undefined;
	export let used = false;

	let clickable = random(true) > 0.5;
	let selected = random(true) > 0.75;
	$: inPlayerArea = !isUndefined(playerId);
</script>

<div class:used style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
	<img
		class="wh-full absolute top-0 left-0 object-cover"
		src={assetUrl(`Boards/Federations/${federationTokenImages.get(type)}.png`)}
		alt=""
	/>
	{#if inPlayerArea}
		<div class={interactiveElementClass(clickable, selected)} on:click />
	{/if}
</div>

<style>
	.used {
		opacity: 0.3;
	}
</style>
