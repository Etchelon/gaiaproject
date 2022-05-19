<script lang="ts">
	import type { ActionSpaceDto } from "$dto/interfaces";
	import { interactiveElementClass } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { random } from "lodash";

	export let space: ActionSpaceDto;

	let clickable = random(true) > 0.5;
	let selected = random(true) > 0.75;
	let notes = false;
	$: elementType =
		space.kind === "power"
			? InteractiveElementType.PowerAction
			: space.kind === "qic"
			? InteractiveElementType.QicAction
			: space.kind === "planetary-institute"
			? InteractiveElementType.PlanetaryInstitute
			: space.kind === "right-academy"
			? InteractiveElementType.RightAcademy
			: InteractiveElementType.RaceAction;
</script>

<div class="action-space">
	{#if !space.isAvailable}
		<div class="token">
			<!-- <ActionToken /> -->
		</div>
	{/if}
	<div class={interactiveElementClass(clickable, selected)} on:click />
	{#if notes}
		<!-- <Tooltip title={notes}>
			<div class="warning">
				<WarningIcon />
			</div>
		</Tooltip> -->
	{/if}
</div>

<style>
	.action-space {
		display: flex;
		align-items: center;
		justify-content: center;
		height: 100%;
		position: relative;
	}

	.token {
		width: 83%;
		position: absolute;
		bottom: 6%;
		left: 8%;
	}

	.warning {
		position: absolute;
		bottom: 5;
		right: 5;
	}
</style>
