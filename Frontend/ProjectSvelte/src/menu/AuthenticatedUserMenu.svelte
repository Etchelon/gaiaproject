<script lang="ts">
	import { logOut } from "ionicons/icons";
	import { getAppContext } from "../App.context";

	const { auth } = getAppContext();
	const { loggedUser, logout } = auth;
</script>

<ion-item href="#/games/active" lines="full">
	<ion-label>
		<h2>Active Games</h2>
	</ion-label>
</ion-item>
<ion-item href="#/games/waiting" lines="full">
	<ion-label>
		<h2>Active Games</h2>
		<p>That are waiting for you</p>
	</ion-label>
</ion-item>
<ion-item href="#/games/finished" lines="full">
	<ion-label>
		<h2>Finished Games</h2>
	</ion-label>
</ion-item>

<ion-item lines="full">
	<ion-avatar slot="start">
		<ion-img src={$loggedUser.avatar} alt="AVT" />
	</ion-avatar>
	<ion-label>
		<h2>{$loggedUser.username}</h2>
		<p>
			<a href="#/profile">Manage profile</a>
		</p>
	</ion-label>
	<ion-button slot="end" fill="none" on:click={logout}>
		<ion-icon slot="icon-only" icon={logOut} />
	</ion-button>
</ion-item>
