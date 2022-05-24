<script lang="ts">
	import { assetUrl } from "$utils/miscellanea";

	export let type: string = "";
	export let imageUrl: string | undefined = undefined;
	export let scale = 1;
</script>

<img class="h-full object-cover" style:transform={`scale(${scale}`} src={assetUrl(imageUrl ?? `Markers/${type}.png`)} alt={type} />
