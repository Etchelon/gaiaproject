<script lang="ts">
	import DesktopView from "./desktop/DesktopView.svelte";
	import StatusBar from "./status-bar/StatusBar.svelte";
	import { getGamePageContext } from "./GamePage.context";

	const { store } = getGamePageContext();

	const isSpectator = false;
	const isMobile = false;
</script>

{#if $store.game}
	<div class="h-screen bg-gray-900">
		<div class="status-bar" class:desktop={!isMobile} class:mobile={isMobile}>
			<StatusBar playerId={null} {isSpectator} {isMobile} />
		</div>
		<div class="game-view" class:desktop={!isMobile} class:mobile={isMobile}>
			{#if !isMobile}
				<DesktopView currentPlayerId="" {isSpectator} />
			{/if}
		</div>
	</div>
{/if}

<style lang="scss">
	.status-bar {
		padding: 3px;

		&.desktop {
			height: 56px;
		}

		&.mobile {
			position: sticky;
			top: 0;
			padding-left: 0;
			padding-right: 0;
			z-index: 11;
			background-color: black;
		}
	}

	.game-view {
		padding: 3px;

		&.desktop {
			height: calc(100% - 56px);
			overflow: hidden;
		}

		&.mobile {
			overflow: auto;
		}
	}
</style>
