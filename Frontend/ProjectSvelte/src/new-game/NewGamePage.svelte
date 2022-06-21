<script lang="ts" context="module">
	const MIN_OTHER_PLAYERS = 1;
	const MAX_OTHER_PLAYERS = 3;
	const DEFAULT_STARTING_VPS = 10;

	type BalancingMethod = "auction" | "sector-rotation";
</script>

<script lang="ts">
	import LoadingSpinner from "$components/LoadingSpinner.svelte";
	import { MapShape, RaceSelectionMode, TurnOrderSelectionMode } from "$dto/enums";
	import type { CreateGameCommand, UserInfoDto } from "$dto/interfaces";
	import type { Nullable } from "$utils/miscellanea";
	import { toastController } from "@ionic/core";
	import type { IonSearchbar } from "@ionic/core/components/ion-searchbar";
	import { trashBinOutline } from "ionicons/icons";
	import { debounce, reject, size, some } from "lodash";
	import { DateTime } from "luxon";
	import { getAppContext } from "../app/App.context";

	export let onStart: (command: CreateGameCommand) => void;
	export let onClose: () => void;

	const { http } = getAppContext();

	let ionSearchBar: IonSearchbar;
	let isSearching = false;
	let searchedUsers$: Promise<UserInfoDto[]> = Promise.resolve([]);
	let hasSearched = false;
	const openSearch = () => {
		isSearching = true;
	};
	const closeSearch = () => {
		isSearching = false;
	};
	let selectedUsers: UserInfoDto[] = [];
	let raceSelectionMode = RaceSelectionMode.TurnOrder;
	let balancingMethod: Nullable<BalancingMethod> = null;
	let startingVPs = DEFAULT_STARTING_VPS;
	const setStartingVPs = (evt: any) => (startingVPs = +evt.target.value);
	let gameName: Nullable<string> = null;
	const setGameName = (evt: any) => (gameName = evt.target.value);
	let isCreating = false;

	const searchUsersImpl = debounce(async (filter: string) => {
		searchedUsers$ = http
			.get<UserInfoDto[]>(`api/Users/Search/${filter}`)
			.then(users => reject(users, u => some(selectedUsers, su => su.id === u.id)))
			.catch(async () => {
				const toast = await toastController.create({
					message: "Error trying to search for users. Try again later",
					color: "danger",
					duration: 3000,
					position: "bottom",
				});
				await toast.present();
				return [];
			})
			.finally(() => {
				hasSearched = true;
			});
	}, 500);
	const searchUsers = ({ target }: any) => {
		const filter = target.value as string;
		if (!filter || size(filter) < 2) {
			return;
		}
		searchUsersImpl(filter);
	};

	const selectUser = (user: Nullable<UserInfoDto>) => {
		if (!user) {
			return;
		}

		selectedUsers = [...selectedUsers, user];
		toastController
			.create({
				message: `${user.username} added to the match`,
				color: "success",
				duration: 1500,
				position: "bottom",
			})
			.then(toast => toast.present());

		if (selectedUsers.length === MAX_OTHER_PLAYERS) {
			closeSearch();
			return;
		}

		hasSearched = false;
		searchedUsers$ = Promise.resolve([]);
		ionSearchBar.value = "";
		ionSearchBar.setFocus();
	};
	const unselectUser = (user: UserInfoDto) => {
		const remainingUsers = reject(selectedUsers, u => u.id === user.id);
		selectedUsers = remainingUsers;
	};

	const onRaceSelectionModeChanged = (evt: any) => {
		const mode: RaceSelectionMode = evt.target.value;
		raceSelectionMode = mode;
		mode === RaceSelectionMode.Random && balancingMethod === "sector-rotation" && (balancingMethod = null);
	};

	const getCommand = () => {
		const command: CreateGameCommand = {
			playerIds: selectedUsers.map(u => u.id),
			options: {
				factionSelectionMode: raceSelectionMode,
				turnOrderSelectionMode: TurnOrderSelectionMode.Random,
				auction: balancingMethod === "auction",
				rotateSectorsInSetup: balancingMethod === "sector-rotation",
				mapShape: selectedUsers.length as MapShape,
				startingVPs,
				gameName: gameName ?? `Gaia Project - ${DateTime.now().toFormat("eee d, MMMM yyyy (HH:mm)")}`,
			},
		};
		return command;
	};
</script>

<ion-header>
	<ion-toolbar>
		<ion-title class="gaia-font">New Game</ion-title>
	</ion-toolbar>
</ion-header>
<ion-content fullscreen>
	<div class="p-1 md:p-3">
		<div class="flex flex-col gap-2 md:gap-4">
			<div>
				<h6 class="gaia-font">Player Selection</h6>
				<ion-button expand="block" color="primary" disabled={selectedUsers.length === MAX_OTHER_PLAYERS} on:click={openSearch}>
					<span class="gaia-font">Search Player</span>
				</ion-button>

				<ion-list class="mt-2">
					{#each selectedUsers as user, index (user.id)}
						<ion-item lines={index < selectedUsers.length - 1}>
							<ion-avatar slot="start">
								<img src={user.avatar} alt={user.username} />
							</ion-avatar>
							<ion-label class="gaia-font">
								{user.username}
							</ion-label>
							<ion-button slot="end" color="warning" on:click={() => unselectUser(user)}>
								<ion-icon slot="icon-only" icon={trashBinOutline} />
							</ion-button>
						</ion-item>
					{:else}
						<ion-item>
							<ion-label class="gaia-font"> Add at least 1 player </ion-label>
						</ion-item>
					{/each}
				</ion-list>
			</div>
			<div>
				<h6 class="gaia-font">Game Options</h6>
				<ion-list>
					<ion-item>
						<ion-label class="gaia-font">Game Name</ion-label>
						<ion-input
							placeholder="Choose a game name (required)"
							type="text"
							required
							value={gameName}
							on:ionChange={setGameName}
						/>
					</ion-item>
					<ion-radio-group value={raceSelectionMode} on:ionChange={onRaceSelectionModeChanged}>
						<ion-list-header>
							<ion-label class="gaia-font">Race Selection Mode</ion-label>
						</ion-list-header>
						<ion-item>
							<ion-label class="gaia-font">Free Choice</ion-label>
							<ion-radio slot="start" value={RaceSelectionMode.TurnOrder} />
						</ion-item>
						<ion-item>
							<ion-label class="gaia-font">Random</ion-label>
							<ion-radio slot="start" value={RaceSelectionMode.Random} />
						</ion-item></ion-radio-group
					>
					<ion-radio-group value={balancingMethod} on:ionChange={onRaceSelectionModeChanged}>
						<ion-list-header>
							<ion-label class="gaia-font">Balancing Method</ion-label>
						</ion-list-header>
						<ion-item>
							<ion-label class="gaia-font">Auction</ion-label>
							<ion-radio slot="start" value="auction" />
						</ion-item>
						<ion-item>
							<ion-label class="gaia-font">Sector Rotation</ion-label>
							<ion-radio slot="start" value="sector-rotation" disabled={raceSelectionMode === RaceSelectionMode.Random} />
						</ion-item></ion-radio-group
					>
					<ion-item>
						<ion-label class="gaia-font">Starting VPs</ion-label>
						<ion-input placeholder="Min 10 VPs" type="number" value={startingVPs} min={10} on:ionChange={setStartingVPs} />
					</ion-item>
				</ion-list>
			</div>
		</div>
	</div>
</ion-content>
<ion-footer>
	<ion-toolbar>
		<ion-buttons slot="end">
			<ion-button disabled={isCreating} on:click={onClose}>
				<span class="gaia-font">Cancel</span>
			</ion-button>
			<ion-button
				color="primary"
				disabled={isCreating || selectedUsers.length < MIN_OTHER_PLAYERS || !gameName}
				on:click={() => onStart(getCommand())}
			>
				<span class="gaia-font">Start</span>
			</ion-button>
		</ion-buttons>
	</ion-toolbar>
</ion-footer>

<ion-modal
	is-open={isSearching}
	on:didPresent={() => {
		setTimeout(() => ionSearchBar.setFocus(), 250);
	}}
	on:didDismiss={closeSearch}
>
	<ion-header>
		<ion-toolbar>
			<ion-title class="gaia-font">Search Player</ion-title>
		</ion-toolbar>
		<ion-toolbar>
			<ion-searchbar
				class="gaia-font"
				debounce={500}
				show-cancel-button="focus"
				inputmode="search"
				bind:this={ionSearchBar}
				on:ionInput={searchUsers}
			/>
		</ion-toolbar>
	</ion-header>
	<ion-content fullscreen>
		{#await searchedUsers$}
			<LoadingSpinner />
		{:then users}
			<ion-list>
				{#each users as user, index (user.id)}
					<ion-item button lines={index < users.length - 1} on:click={() => selectUser(user)}>
						<ion-avatar slot="start">
							<img src={user.avatar} alt={user.username} />
						</ion-avatar>
						<ion-label class="gaia-font">
							{user.username}
						</ion-label>
					</ion-item>
				{/each}
				{#if hasSearched && users.length === 0}
					<ion-item>
						<ion-label class="gaia-font">No players found</ion-label>
					</ion-item>
				{/if}
			</ion-list>
		{/await}
	</ion-content>
	<ion-footer>
		<ion-toolbar>
			<ion-buttons slot="end">
				<ion-button on:click={closeSearch}>
					<span class="gaia-font">Done</span>
				</ion-button>
			</ion-buttons>
		</ion-toolbar>
	</ion-footer>
</ion-modal>
