<script lang="ts" context="module">
	import { NotificationType } from "$dto/enums";
	import type { NotificationDto } from "$dto/interfaces";
	import GameNotification from "./GameNotification.svelte";
	import GenericNotification from "./GenericNotification.svelte";

	const componentMap = new Map<NotificationType, any>([
		[NotificationType.Game, GameNotification],
		[NotificationType.Generic, GenericNotification],
	]);
</script>

<script lang="ts">
	export let notification: NotificationDto;
	export let onRead: () => void;

	$: component = componentMap.get(notification.type) ?? GenericNotification;
</script>

<svelte:component this={component} {notification} {onRead} />
