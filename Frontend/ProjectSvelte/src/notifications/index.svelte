<script lang="ts">
	import type { NotificationDto } from "$dto/interfaces";
	import { App } from "@capacitor/app";
	import type { PluginListenerHandle } from "@capacitor/core";
	import { mailOutline } from "ionicons/icons";
	import { last } from "lodash";
	import { DateTime } from "luxon";
	import { onDestroy, onMount } from "svelte";
	import { getAppContext } from "../app/App.context";
	import Notification from "./Notification.svelte";

	const { http } = getAppContext();
	const PAGE_SIZE = 10;

	let showNotifications = false;
	let unreadNotificationsCount = 0;
	let notifications: NotificationDto[] = [];
	let hasLoadedNotifications = false;
	let hasMoreNotifications = true;
	const fetchCount = async () => (unreadNotificationsCount = await http.get<number>("api/Users/CountUnreadNotifications"));
	const loadNotifications = async (loadMore = false) => {
		if (!loadMore && hasLoadedNotifications) {
			return;
		}

		const earliestLoadedNotificationIsoDate = last(notifications)?.timestamp;
		const earliestNotificationDate = earliestLoadedNotificationIsoDate
			? DateTime.fromISO(earliestLoadedNotificationIsoDate).toJSDate()
			: new Date();

		try {
			const isoEarlierThan = earliestNotificationDate.toISOString();
			const notifications_ = await http.get<NotificationDto[]>(`api/Users/GetUserNotifications?earlierThan=${isoEarlierThan}`);
			notifications = [...notifications, ...notifications_];
			hasMoreNotifications = notifications.length === PAGE_SIZE;
		} catch (err) {
			console.error({ err });
		} finally {
			hasLoadedNotifications = true;
		}
	};
	const setNotificationRead = (notification: NotificationDto) => {
		unreadNotificationsCount = Math.max(unreadNotificationsCount - 1, 0);
		notification.isRead = true;
		notifications = [...notifications];
		http.put(`api/Users/SetNotificationRead/${notification.id}`);
	};

	let appStateChangeListener: PluginListenerHandle;
	onMount(async () => {
		await fetchCount();
		appStateChangeListener = await App.addListener("appStateChange", async ({ isActive }) => {
			const THIRTY_SECONDS = 30000;
			let fetchCountInterval: number | undefined;
			if (isActive) {
				fetchCount();
				window.clearInterval(fetchCountInterval);
				fetchCountInterval = window.setInterval(async () => {
					await fetchCount();
				}, THIRTY_SECONDS);
			} else {
				window.clearInterval(fetchCountInterval);
				fetchCountInterval = undefined;
			}
		});
	});

	onDestroy(async () => {
		await appStateChangeListener.remove();
	});

	$: {
		showNotifications && loadNotifications();
	}
</script>

<div class="relative">
	<ion-button
		id="trigger"
		class="btn"
		class:relative={unreadNotificationsCount > 0}
		shape="round"
		on:click={() => {
			showNotifications = !showNotifications;
		}}
	>
		<ion-icon slot="icon-only" icon={mailOutline} />
	</ion-button>
	{#if unreadNotificationsCount > 0}
		<ion-badge class="badge absolute" color="danger">{unreadNotificationsCount}</ion-badge>
	{/if}
</div>
<ion-popover
	trigger="trigger"
	reference="trigger"
	side="bottom"
	alignment="end"
	backdrop-dismiss={true}
	dismiss-on-select={true}
	is-open={showNotifications}
	on:didDismiss={() => {
		showNotifications = false;
	}}
>
	<ion-list>
		{#each notifications as notification (notification.id)}
			<Notification {notification} onRead={() => setNotificationRead(notification)} />
		{:else}
			{#if hasLoadedNotifications}
				<ion-item>
					<ion-label class="gaia-font"> No notifications to show! </ion-label>
				</ion-item>
			{/if}
		{/each}
		{#if hasMoreNotifications}
			<ion-button expand="block" color="dark" on:click|stopPropagation={() => loadNotifications(true)}>
				<small class="gaia-font">Load more...</small>
			</ion-button>
		{/if}
	</ion-list>
</ion-popover>

<style>
	.btn {
		left: -3px;
	}

	.badge {
		bottom: -2px;
		right: -1px;
		padding: 2px 5px;
		border-radius: 50%;
	}

	ion-popover {
		--width: 75vw;
		--max-width: 400px;
	}
</style>
