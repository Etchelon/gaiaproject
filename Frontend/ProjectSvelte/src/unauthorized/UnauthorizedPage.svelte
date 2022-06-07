<script lang="ts">
	import { getAppContext } from "../app/App.context";

	const { auth } = getAppContext();
</script>

<div class="ion-page">
	<ion-header translucent>
		<ion-toolbar>
			<ion-buttons slot="start">
				<ion-back-button />
			</ion-buttons>
			<ion-title>Unauthorized</ion-title>
		</ion-toolbar>
	</ion-header>
	<ion-content class="flex flex-col justify-center items-center ion-padding" fullscreen>
		<ion-img
			class="gandalf"
			src="https://149360821.v2.pressablecdn.com/wp-content/uploads/2017/12/You-Shall-Not-Pass.jpg"
			alt="You Shall Not Pass"
		/>
		<ion-text color="medium">
			<h1 class="mt-4 text-center cursor-pointer" on:click={auth.login}>You need to login in order to browse the selected content</h1>
		</ion-text>
	</ion-content>
</div>

<style>
	.gandalf {
		height: 75vh;
	}
</style>
