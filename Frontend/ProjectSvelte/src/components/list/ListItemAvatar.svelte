<script lang="ts">
	export let src: string;
</script>

<div class="list-avatar mr-4 aspect-square rounded-full flex justify-center items-center">
	<img class="object-cover w-full" {src} alt="" />
</div>

<style>
	:root {
		--size: 40px;
	}

	.list-avatar {
		width: var(--size);
		height: var(--size);
	}
</style>
