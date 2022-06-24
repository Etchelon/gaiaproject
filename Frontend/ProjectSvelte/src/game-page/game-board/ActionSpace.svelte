<script lang="ts">
	import type { ActionSpaceDto } from "$dto/interfaces";
	import { interactiveElementClass } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { noop } from "lodash";
	import { getGamePageContext } from "../GamePage.context";
	import { deriveActionSpaceInteractionState } from "../store/selectors";
	import ActionToken from "./ActionToken.svelte";

	export let space: ActionSpaceDto;

	const { store, activeWorkflow } = getGamePageContext();
	const elementType =
		space.kind === "power"
			? InteractiveElementType.PowerAction
			: space.kind === "qic"
			? InteractiveElementType.QicAction
			: space.kind === "planetary-institute"
			? InteractiveElementType.PlanetaryInstitute
			: space.kind === "right-academy"
			? InteractiveElementType.RightAcademy
			: InteractiveElementType.RaceAction;
	const interactionState = deriveActionSpaceInteractionState(elementType)(space.type)(store);
	$: ({ clickable, selected, notes } = $interactionState);
	$: actionSpaceClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(space.type, elementType);
		  }
		: noop;
</script>

<div class="action-space">
	{#if !space.isAvailable}
		<div class="token">
			<ActionToken />
		</div>
	{/if}
	<div class={interactiveElementClass(clickable, selected)} on:click={actionSpaceClicked} />
	{#if notes}
		<!-- TODO <Tooltip title={notes}>
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
		bottom: 7%;
		left: 8%;
	}

	.warning {
		position: absolute;
		bottom: 5;
		right: 5;
	}
</style>
