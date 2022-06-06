<script lang="ts">
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { random } from "lodash";

	export let player: PlayerInGameDto;
	export let index: number;
	export let isOnline = false;
</script>

<div class="player-box bg-white">
	<div class="header text-gray-900">
		<img class="avatar" class:opacity-0={!player.isActive} src={assetUrl("PlayerLoader.gif")} alt="" />
		<div class="info">
			<p class="username text-base gaia-font ellipsify">
				{player.username}
			</p>
			<div class="race-name">
				<div class="connection-status" style:background-color={isOnline ? "green" : "grey"} />
				<p class="text-sm gaia-font">No race selected</p>
			</div>
		</div>
		<div class="stats">
			<p class="order ordinal text-base gaia-font">{index}</p>
			<div class="points">
				<span class="gaia-font">-</span>
				<img class="vp-img" src={assetUrl("Markers/VP.png")} alt="" />
			</div>
		</div>
	</div>
</div>

<style lang="scss">
	$spacingHalf: 4px;
	$spacing1: 8px;
	$spacing2: 16px;
	$spacing3: 24px;

	.player-box {
		width: 100%;
		min-width: 250px;
		padding: $spacing1;

		&.has-passed {
			opacity: 0.7;
		}
	}

	.header {
		display: flex;
		align-items: flex-start;
		width: 100%;
		height: 50px;
		margin-bottom: $spacing1;

		.avatar {
			flex-shrink: 0;
			height: 100%;
			object-fit: contain;
			margin-right: $spacing1;
		}

		.img-placeholder {
			height: 100%;
			padding-left: 100%;
		}

		.info {
			flex: 1 1 auto;
			display: flex;
			flex-direction: column;
			min-width: 0;

			.race-name {
				margin-top: auto;
				display: flex;
				align-items: center;

				.connection-status {
					width: 15px;
					height: 15px;
					border-radius: 50%;
					margin-right: 0.5rem;
				}
			}
		}

		.stats {
			flex-shrink: 0;
			display: flex;
			flex-direction: column;
			text-align: end;

			.points {
				display: flex;
				align-items: center;
				margin-top: auto;

				.vp-img {
					height: 1.5rem;
					object-fit: contain;
					margin-left: $spacingHalf;
				}
			}
		}
	}

	.player-data {
		padding: $spacingHalf $spacing1;
		background-color: white;
		color: black;

		.resource-indicator {
			display: flex;
			align-items: center;

			> div {
				width: 35px;
				margin-right: $spacingHalf;
			}

			> span {
				font-size: 0.75rem;
			}
		}

		.maxed-out > span {
			color: rgb(210, 19, 24);
		}

		.fixed-height {
			height: 35px;
		}

		.invisible {
			visibility: hidden;
		}

		.row {
			display: flex;
			align-items: center;
			justify-content: space-between;
			height: 30px;
		}

		.row-spacer {
			width: 100%;
			margin-top: $spacingHalf;
		}
	}
</style>
