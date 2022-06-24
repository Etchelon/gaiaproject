<script lang="ts">
	import Page from "$components/Page.svelte";
	import { getAppContext } from "../app/App.context";

	const { auth } = getAppContext();
</script>

<Page title="Unauthorized">
	<div class="ion-padding">
		<ion-img
			class="gandalf"
			src="https://149360821.v2.pressablecdn.com/wp-content/uploads/2017/12/You-Shall-Not-Pass.jpg"
			alt="You Shall Not Pass"
		/>
		<ion-text color="medium">
			<h1 class="mt-4 text-center cursor-pointer" on:click={auth.login}>
				You need to <span class="underline text-primary-500">login</span> in order to browse the selected content
			</h1>
		</ion-text>
	</div>
</Page>

<style>
	.gandalf {
		height: 75vh;
	}
</style>
