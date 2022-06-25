<script lang="ts">
	import type { GameNotificationDto } from "$dto/interfaces";
	import { checkmarkDoneOutline, diceOutline } from "ionicons/icons";

	export let notification: GameNotificationDto;
	export let onRead: (id: string) => void;
</script>

<ion-item href={`#/game/${notification.gameId}`} on:click={() => onRead(notification.id)}>
	<ion-icon slot="start" icon={diceOutline} />
	<ion-label class="ion-text-wrap gaia-font">
		<small>{notification.text}</small>
	</ion-label>
	{#if notification.isRead}
		<ion-icon slot="end" icon={checkmarkDoneOutline} />
	{/if}
</ion-item>
