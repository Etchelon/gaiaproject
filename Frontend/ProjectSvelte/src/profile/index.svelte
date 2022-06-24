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
			message: "Profile updated!",
			color: "success",
			duration: 3000,
			position: "bottom",
		});
		await toast.present();
	};
</script>

<Page title="Profile">
	<div class="p-1 md:p-3">
		<div class="lg:container lg:mx-auto">
			<div class="flex flex-col items-center">
				<ion-avatar class="user-img mb-2 md:mb-4">
					<ion-img class="user-img" src={user.avatar ?? "/assets/person.png"} alt="" />
				</ion-avatar>
				<ion-list class="self-stretch" lines="full">
					<ion-item>
						<ion-label class="gaia-font" position="stacked">Choose an avatar</ion-label>
						<ion-input type="text" value={user.avatar} on:ionChange={dispatch("avatar")} />
					</ion-item>
					<ion-item>
						<ion-label class="gaia-font" position="stacked">Choose your username</ion-label>
						<ion-input type="text" value={user.username} on:ionChange={dispatch("username")} />
					</ion-item>
					<ion-item>
						<ion-label class="gaia-font" position="stacked">Your first name</ion-label>
						<ion-input type="text" value={user.firstName} on:ionChange={dispatch("firstName")} />
					</ion-item>
					<ion-item>
						<ion-label class="gaia-font" position="stacked">Your last name</ion-label>
						<ion-input type="text" value={user.lastName} on:ionChange={dispatch("lastName")} />
					</ion-item>
				</ion-list>
			</div>
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
