<script lang="ts">
	import type { GameNotificationDto } from "$dto/interfaces";
	import { checkmarkDoneOutline, diceOutline } from "ionicons/icons";
	import { DateTime } from "luxon";

	export let notification: GameNotificationDto;
	export let onRead: () => void;
</script>

<ion-item href={`#/game/${notification.gameId}`} on:click={onRead}>
	<ion-icon slot="start" icon={diceOutline} />
	<ion-label class="ion-text-wrap gaia-font">
		<ion-text color="tertiary">
			<small>{DateTime.fromISO(notification.timestamp).toLocaleString()}</small>
		</ion-text>
		<small>{notification.text}</small>
	</ion-label>
	{#if notification.isRead}
		<ion-icon slot="end" icon={checkmarkDoneOutline} color="success" />
	{/if}
</ion-item>
