<script lang="ts" context="module">
	import type { UserInfoDto } from "$dto/interfaces";

	interface ActionType {
		property: keyof UserInfoDto | "set";
		value: any;
	}

	const reducer = (state: UserInfoDto, action: ActionType): UserInfoDto => {
		if (action.property === "set") {
			return cloneDeep(action.value as UserInfoDto);
		}

		const ret = cloneDeep(state);
		ret[action.property] = action.value;
		return ret;
	};
</script>

<script lang="ts">
	import Page from "$components/Page.svelte";
	import { toastController } from "@ionic/core";
	import { cloneDeep } from "lodash";
	import { get } from "svelte/store";
	import { getAppContext } from "../app/App.context";

	const { http, auth } = getAppContext();

	let user = cloneDeep(get(auth.loggedUser));
	const dispatch = (property: ActionType["property"]) => (evt: any) => (user = reducer(user, { property, value: evt.target.value }));

	const updateProfile = async () => {
		await http.put(`api/Users/UpdateProfile/${user.id}`, user, { readAsString: true });
		auth.updateUser(user);
		const toast = await toastController.create({
			message: "Error trying to search for users. Try again later",
			color: "danger",
			duration: 3000,
			position: "bottom",
		});
		await toast.present();
	};
</script>

<Page title="Profile">
	<div class="p-1 md:p-3">
		<div class="grid grid-cols-2 gap-2 md:gap-4">
			<div class="col-span-2 md:col-span-1">
				<div class="flex flex-col items-center">
					<ion-avatar class="user-img">
						<img class="user-img" src={user.avatar ?? "/assets/person.png"} alt="" />
					</ion-avatar>
					<ion-item>
						<ion-label class="gaia-font">Choose and avatar</ion-label>
						<ion-input type="text" value={user.avatar} on:ionChange={dispatch("avatar")} />
					</ion-item>
					<ion-item>
						<ion-label class="gaia-font">Choose your username</ion-label>
						<ion-input type="text" value={user.avatar} on:ionChange={dispatch("username")} />
					</ion-item>
				</div>
			</div>
			<div class="col-span-2 md:col-span-1" />
		</div>
	</div>
	<ion-footer slot="footer">
		<ion-toolbar>
			<ion-buttons slot="end">
				<ion-button color="primary" on:click={updateProfile}>
					<span class="gaia-font">Save</span>
				</ion-button>
			</ion-buttons>
		</ion-toolbar>
	</ion-footer>
</Page>

<style>
	.user-img {
		width: 200px;
		height: 200px;
	}
</style>
