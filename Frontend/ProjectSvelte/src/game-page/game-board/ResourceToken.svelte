<script lang="ts">
	import { assetUrl } from "$utils/miscellanea";

	export let type: string;
	export let imageUrl: string | undefined;
	export let scale = 1;
</script>

<div class="fill-parent centered-flex-row">
	<img
		class="fill-parent"
		style:object-fit="cover"
		style:transform={`scale(${scale}`}
		src={assetUrl(imageUrl ?? `Markers/${type}.png`)}
		alt={type}
	/>
</div>
