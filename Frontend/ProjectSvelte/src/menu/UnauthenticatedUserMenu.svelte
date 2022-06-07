<script lang="ts">
	import { logIn } from "ionicons/icons";
	import { getAppContext } from "../app/App.context";

	const { auth } = getAppContext();
	const { login } = auth;
</script>

<div class="flex-auto overflow-x-hidden overflow-y-auto">
	<!-- No routes -->
</div>

<ion-item class="flex-shrink-0" lines="none">
	<ion-label>
		<h2>Anonymous user</h2>
		<p>Login to use all the features</p>
	</ion-label>
	<ion-button slot="end" fill="none" on:click={login}>
		<ion-icon slot="icon-only" icon={logIn} />
	</ion-button>
</ion-item>
