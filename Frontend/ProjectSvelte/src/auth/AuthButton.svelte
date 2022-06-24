<script lang="ts">
	import type { UserInfoDto } from "$dto/interfaces";
	import { logIn } from "ionicons/icons";
	import { logOut } from "ionicons/icons";
	import { get } from "svelte/store";
	import { getAppContext } from "../app/App.context";

	const { auth } = getAppContext();
	const { isAuthenticated, loggedUser, login, logout } = auth;
	let user: UserInfoDto;
	$: {
		if ($isAuthenticated) {
			user = get(loggedUser);
		}
	}
</script>

{#if $isAuthenticated}
	<ion-item lines="none">
		<ion-avatar slot="start">
			<ion-img src={user.avatar} alt="AVT" />
		</ion-avatar>
		<ion-label>
			<h2>{user.username}</h2>
			<p>
				<a href="#/profile">Manage profile</a>
			</p>
		</ion-label>
		<ion-button slot="end" fill="none" on:click={logout}>
			<ion-icon slot="icon-only" icon={logOut} />
		</ion-button>
	</ion-item>
{:else}
	<ion-item lines="none" on:click={login}>
		<ion-label>
			<h2>Anonymous user</h2>
			<p>Login to use all the features</p>
		</ion-label>
		<ion-button slot="end" fill="none">
			<ion-icon slot="icon-only" icon={logIn} />
		</ion-button>
	</ion-item>
{/if}
