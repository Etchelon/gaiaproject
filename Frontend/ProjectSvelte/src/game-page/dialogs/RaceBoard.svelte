<script lang="ts" context="module">
	const boardVersion = "rework";
</script>

<script lang="ts">
	import type { Race } from "$dto/enums";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceBoard, getRaceName } from "$utils/race-utils";

	export let race: Race;
</script>

<img class="w-100" src={assetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(race)}`)} alt={`${getRaceName(race)}'s board`} />
