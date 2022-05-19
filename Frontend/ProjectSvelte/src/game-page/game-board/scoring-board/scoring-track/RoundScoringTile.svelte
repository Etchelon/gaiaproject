<script lang="ts" context="module">
	import { RoundScoringTileType } from "$dto/enums";

	const WIDTH_TO_HEIGHT_RATIO = 0.876;
	const roundScoringTileNames = new Map<RoundScoringTileType, string>([
		[RoundScoringTileType.PointsPerTerraformingStep2, "RNDter"],
		[RoundScoringTileType.PointsPerResearchStep2, "RNDstp"],
		[RoundScoringTileType.PointsPerMine2, "RNDmin"],
		[RoundScoringTileType.PointsPerTradingStation3, "RNDtrs3"],
		[RoundScoringTileType.PointsPerTradingStation4, "RNDtrs4"],
		[RoundScoringTileType.PointsPerGaiaPlanet3, "RNDgai3"],
		[RoundScoringTileType.PointsPerGaiaPlanet4, "RNDgai4"],
		[RoundScoringTileType.PointsPerBigBuilding5, "RNDpia"],
		[RoundScoringTileType.PointsPerBigBuilding5Bis, "RNDpia"],
		[RoundScoringTileType.PointsPerFederation5, "RNDfed"],
	]);
</script>

<script lang="ts">
	import type { RoundScoringTileDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";

	export let tile: RoundScoringTileDto;
	export let width: number;

	$: style = `width: ${width}px; height: ${width / WIDTH_TO_HEIGHT_RATIO}px; opacity: ${tile.inactive ? 0.3 : 1}`;
</script>

<div {style}>
	<img class="fill-parent" src={assetUrl(`Boards/ScoringTiles/${roundScoringTileNames.get(tile.tileId)}.png`)} alt="" />
</div>
