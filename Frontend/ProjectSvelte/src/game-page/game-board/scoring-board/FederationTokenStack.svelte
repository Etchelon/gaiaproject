<script lang="ts" context="module">
	const HEIGHT_TO_WIDTH_RATIO = 1.05;
</script>

<script lang="ts">
	import type { FederationTokenStackDto } from "$dto/interfaces";
	import { interactiveElementClass, withAspectRatioW } from "$utils/miscellanea";
	import { InteractiveElementType } from "$utils/types";
	import { noop, range } from "lodash";
	import { getGamePageContext } from "../../GamePage.context";
	import { deriveFederationTokenStackInteractionState } from "../../store/selectors";
	import FederationToken from "../FederationToken.svelte";

	export let stack: FederationTokenStackDto;

	const { store, activeWorkflow } = getGamePageContext();
	const interactionState = deriveFederationTokenStackInteractionState(stack.type)(store);
	$: ({ clickable, selected } = $interactionState);
	$: stackClicked = clickable
		? () => {
				$activeWorkflow?.elementSelected(stack.type, InteractiveElementType.FederationToken);
		  }
		: noop;
</script>

<div style={withAspectRatioW(1 / HEIGHT_TO_WIDTH_RATIO)}>
	<div class="wh-full absolute top-0 left-0">
		{#each range(0, stack.remaining) as n}
			<div class="token" style={`top: ${n * 12}%; left: ${n * 15}%`}>
				<FederationToken type={stack.type} />
			</div>
		{/each}
	</div>
	<div class={interactiveElementClass(clickable, selected)} on:click={stackClicked} />
</div>

<style>
	.token {
		position: absolute;
		width: 70%;
	}
</style>
