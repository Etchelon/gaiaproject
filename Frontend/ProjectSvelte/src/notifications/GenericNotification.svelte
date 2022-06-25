<script lang="ts">
	import type { GameNotificationDto } from "$dto/interfaces";
	import { checkmarkDoneOutline, informationCircleOutline } from "ionicons/icons";

	export let notification: GameNotificationDto;
	export let onRead: () => void;
</script>

<ion-item on:click={onRead}>
	<ion-icon slot="start" icon={informationCircleOutline} />
	<ion-label class="ion-text-wrap gaia-font">
		<small>{notification.text}</small>
	</ion-label>
	{#if notification.isRead}
		<ion-icon slot="end" icon={checkmarkDoneOutline} />
	{/if}
</ion-item>
