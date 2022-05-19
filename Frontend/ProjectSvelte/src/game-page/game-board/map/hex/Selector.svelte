<script lang="ts">
	export let radius: number;
	export let selected = false;
	export let withQic = false;
</script>

<svg width={radius} height={radius} fill="transparent" xmlns="http://www.w3.org/2000/svg" on:click>
	<circle class="selector" class:selected class:with-qic={withQic} cx={radius / 2} cy={radius / 2} r={radius / 2 - 4} />
</svg>

<style>
	.selector {
		stroke: var(--selectable-color);
		stroke-width: 2px;
	}

	.selector.selected {
		stroke: var(--selected-color);
	}

	.selector.with-qic {
		stroke: var(--qic-color);
	}
</style>
