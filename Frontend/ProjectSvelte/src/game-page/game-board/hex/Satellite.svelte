<script lang="ts">
	import type { Race } from "../../../dto/enums";
	import { getRaceColor } from "../../../utils/race-utils";

	export let raceId: Race;
	export let width: number;

	$: styles = {
		width: `${width}px`,
		height: `${width}px`,
		backgroundColor: getRaceColor(raceId),
	};
</script>

<div class="satellite" style:width={styles.width} style:height={styles.height} style:background-color={styles.backgroundColor} />

<style>
	.satellite {
		border: 2px solid darkgray;
		border-radius: 3px;
	}
</style>
