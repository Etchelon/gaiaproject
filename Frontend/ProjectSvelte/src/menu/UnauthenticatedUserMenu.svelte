<script lang="ts">
	import { logIn } from "ionicons/icons";
	import { getAppContext } from "../App.context";

	const { auth } = getAppContext();
	const { login } = auth;
</script>

<ion-item lines="full">
	<ion-label>
		<h2>Anonymous user</h2>
		<p>Login to use all the features</p>
	</ion-label>
	<ion-button slot="end" fill="none" on:click={login}>
		<ion-icon slot="icon-only" icon={logIn} />
	</ion-button>
</ion-item>
