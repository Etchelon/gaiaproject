<script lang="ts">
	import { logIn } from "ionicons/icons";
	import { getAppContext } from "../app/App.context";

	const { auth } = getAppContext();
	const { login } = auth;
</script>

<div class="p-2 md:p-4">
	<ion-text class="gaia-font" color="secondary">
		<h1>Welcome to Star Gate!</h1>
	</ion-text>
	<ion-button class="mt-3" expand="block" color="primary" on:click={login}>
		<ion-icon slot="start" icon={logIn} />
		<ion-label class="gaia-font">Login to play!</ion-label>
	</ion-button>
</div>
