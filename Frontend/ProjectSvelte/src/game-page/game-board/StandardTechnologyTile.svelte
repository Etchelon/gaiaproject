<script lang="ts" context="module">
	import { StandardTechnologyTileType } from "$dto/enums";

	const WIDTH_TO_HEIGHT_RATIO = 1.328;
	const standardTileImages = new Map<StandardTechnologyTileType, string>([
		[StandardTechnologyTileType.ActionGain4Power, "TECpow"],
		[StandardTechnologyTileType.Immediate1Ore1Qic, "TECqic"],
		[StandardTechnologyTileType.Immediate1KnowledgePerPlanetType, "TECtyp"],
		[StandardTechnologyTileType.Immediate7Points, "TECvps"],
		[StandardTechnologyTileType.Income1Ore1Power, "TECore"],
		[StandardTechnologyTileType.Income1Knowledge1Coin, "TECknw"],
		[StandardTechnologyTileType.Income4Coins, "TECcre"],
		[StandardTechnologyTileType.PassiveBigBuildingsWorth4Power, "TECpia"],
		[StandardTechnologyTileType.Passive3PointsPerGaiaPlanet, "TECgai"],
	]);
</script>

<script lang="ts">
	import { assetUrl, interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { noop } from "lodash";
	import { getGamePageContext } from "../GamePage.context";
	import { deriveStandardTileInteractionState } from "../store/selectors";

	export let type: StandardTechnologyTileType;

	const { store, activeWorkflow } = getGamePageContext();
	const interactionState = deriveStandardTileInteractionState(type)(store);
	$: ({ clickable, selected } = $interactionState);
	$: tileClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(type, InteractiveElementType.StandardTile);
		  }
		: noop;
</script>

<div style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
	<img class="wh-full absolute top-0 left-0" src={assetUrl(`Boards/TechTiles/${standardTileImages.get(type)}.png`)} alt="" />
	<div class={interactiveElementClass(clickable, selected)} on:click={tileClicked} />
</div>
