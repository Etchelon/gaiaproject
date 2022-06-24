<script lang="ts" context="module">
	import { AdvancedTechnologyTileType } from "$dto/enums";

	const WIDTH_TO_HEIGHT_RATIO = 1.328;
	const advancedTileImages = new Map<AdvancedTechnologyTileType, string>([
		[AdvancedTechnologyTileType.ActionGain1Qic5Credits, "ADVqic"],
		[AdvancedTechnologyTileType.ActionGain3Ores, "ADVore"],
		[AdvancedTechnologyTileType.ActionGain3Knowledge, "ADVknw"],
		[AdvancedTechnologyTileType.Immediate2PointsPerMine, "ADVminV"],
		[AdvancedTechnologyTileType.Immediate4PointsPerTradingStation, "ADVtrsV"],
		[AdvancedTechnologyTileType.Immediate5PointsPerFederation, "ADVfedV"],
		[AdvancedTechnologyTileType.Immediate2PointsPerSector, "ADVsecV"],
		[AdvancedTechnologyTileType.Immediate2PointsPerGaiaPlanet, "ADVgai"],
		[AdvancedTechnologyTileType.Immediate1OrePerSector, "ADVsecO"],
		[AdvancedTechnologyTileType.Pass3PointsPerFederation, "ADVfedP"],
		[AdvancedTechnologyTileType.Pass3PointsPerResearchLab, "ADVlab"],
		[AdvancedTechnologyTileType.Pass1PointsPerPlanetType, "ADVtyp"],
		[AdvancedTechnologyTileType.Passive2PointsPerResearchStep, "ADVstp"],
		[AdvancedTechnologyTileType.Passive3PointsPerMine, "ADVminB"],
		[AdvancedTechnologyTileType.Passive3PointsPerTradingStation, "ADVtrsB"],
	]);
</script>

<script lang="ts">
	import { assetUrl, interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { noop } from "lodash";
	import { getGamePageContext } from "../GamePage.context";
	import { deriveAdvancedTileInteractionState } from "../store/selectors";
	import { InteractiveElementType } from "../workflows/enums";

	export let type: AdvancedTechnologyTileType;

	const { store, activeWorkflow } = getGamePageContext();
	const interactionState = deriveAdvancedTileInteractionState(type)(store);
	$: ({ clickable, selected } = $interactionState);
	$: tileClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(type, InteractiveElementType.AdvancedTile);
		  }
		: noop;
</script>

<div style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
	<img class="wh-full absolute top-0 left-0" src={assetUrl(`Boards/TechTiles/${advancedTileImages.get(type)}.png`)} alt="" />
	<div class={interactiveElementClass(clickable, selected)} on:click={tileClicked} />
</div>
