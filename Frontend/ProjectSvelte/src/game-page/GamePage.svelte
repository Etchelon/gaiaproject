<script lang="ts">
	import DesktopView from "./desktop/DesktopView.svelte";
	import StatusBar from "./status-bar/StatusBar.svelte";
	import { getGamePageContext } from "./GamePage.context";

	const { store } = getGamePageContext();

	const isSpectator = false;
	const isMobile = false;
	const players: any[] = [];
	$: game = $store.game;
	$: activeView = $store.activeView;
</script>

{#if game}
	<div>
		<div>
			<div>
				<StatusBar {game} playerId={null} {isSpectator} {isMobile} />
			</div>
			<div>
				{#if !isMobile}
					<DesktopView {game} currentPlayerId="" {isSpectator} {players} {activeView} />
				{/if}
			</div>
		</div>
	</div>
{/if}
