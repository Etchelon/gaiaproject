<script lang="ts">
	export let text: string;
	export let size: "xs" | "sm" | "base" = "base";
	export let classes: string | null = null;
</script>

<p class={`flex-auto min-w-0 my-1 gaia-font break-words text-${size}${classes ? ` ${classes}` : ""}`}>{text}</p>
