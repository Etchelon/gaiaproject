<script lang="ts" context="module">
	import type { PowerPoolsDto } from "$dto/interfaces";
	import { memoize } from "lodash";

	const MAX_ORES_KNOWLEDGE = 15;
	const MAX_CREDITS = 30;
	const MAX_DEPLOYABLE_MINES = 8;
	const MAX_DEPLOYABLE_TRADING_STATIONS = 4;
	const MAX_DEPLOYABLE_RESEARCH_LABS = 3;
	const MAX_RESEARCH_STEPS = 5;

	const getPowerPoolsSummary = memoize((powerPools: PowerPoolsDto): string => {
		const bowl1 = powerPools.bowl1 + (powerPools.brainstone === 1 ? " (B)" : "");
		const bowl2 = powerPools.bowl2 + (powerPools.brainstone === 2 ? " (B)" : "");
		const bowl3 = powerPools.bowl3 + (powerPools.brainstone === 3 ? " (B)" : "");
		const gaia = `Gaia: ${powerPools.gaiaArea}` + (powerPools.brainstone === 4 ? " (B)" : "");
		return `${bowl1}/${bowl2}/${bowl3} (${gaia})`;
	});
</script>

<script lang="ts">
	import { BuildingType, Race } from "$dto/enums";
	import type { PlayerInGameDto } from "$dto/interfaces";
	import { assetUrl, countActivatableActions } from "$utils/miscellanea";
	import { isNil } from "lodash";
	import { getRaceColors, getRaceImage, getRaceName } from "$utils/race-utils";
	import ResourceToken from "../ResourceToken.svelte";
	import Building from "../map/hex/Building.svelte";

	export let player: PlayerInGameDto;
	export let index: number;
	export let isOnline = false;

	$: [color, textColor] = getRaceColors(player.raceId);
	const { available: availableSpecialActions, all: allSpecialActions } = countActivatableActions(player, false);
	const playerState = player.state!;
	const hasLostPlanet = playerState.researchAdvancements.navigation === MAX_RESEARCH_STEPS;
	const showNextRoundTurnOrder = playerState.hasPassed && playerState.nextRoundTurnOrder !== null;
	const imgUrl = assetUrl(`Races/${getRaceImage(player.raceId)}`);
</script>

<div class="player-box" class:has-passed={player.state.hasPassed} style:background-color={color}>
	<div class="header" style:color={textColor}>
		<img class="avatar" src={player.isActive ? assetUrl("PlayerLoader.gif") : imgUrl} alt="" />
		<div class="info">
			<p class="username text-base gaia-font ellipsify">
				{player.username}
			</p>
			<div class="race-name">
				<div class="connection-status" style:background-color={isOnline ? "green" : "grey"} />
				<p class="text-sm gaia-font">{getRaceName(player.raceId ?? 0)}</p>
			</div>
		</div>
		<div class="stats">
			<p class="order text-base gaia-font">
				{showNextRoundTurnOrder ? `N: ${playerState.nextRoundTurnOrder}°` : `${playerState.currentRoundTurnOrder}°`}
			</p>
			<div class="points">
				<span class="gaia-font"
					>{playerState.points + ((playerState.auctionPoints ?? 0) > 0 ? ` (-${playerState.auctionPoints})` : "")}</span
				>
				<img class="vp-img" src={assetUrl("Markers/VP.png")} alt="" />
			</div>
		</div>
	</div>
	<div class="player-data">
		<div class="row">
			<div class="resource-indicator" class:maxed-out={playerState.resources.ores === MAX_ORES_KNOWLEDGE}>
				<ResourceToken type="Ores" />
				<span class="gaia-font">{playerState.resources.ores}</span>
			</div>
			<div class="resource-indicator" class:maxed-out={playerState.resources.credits === MAX_CREDITS}>
				<ResourceToken type="Credits" />
				<span class="gaia-font">{playerState.resources.credits}</span>
			</div>
			<div class="resource-indicator" class:maxed-out={playerState.resources.knowledge === MAX_ORES_KNOWLEDGE}>
				<ResourceToken type="Knowledge" />
				<span class="gaia-font">{playerState.resources.knowledge}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Qic" />
				<span class="gaia-font">{playerState.resources.qic}</span>
			</div>
		</div>
		<div class="row">
			<div class="resource-indicator">
				<ResourceToken type="Hand" />
				<span class="gaia-font">{playerState.income.ores}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Hand" />
				<span class="gaia-font">{playerState.income.credits}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Hand" />
				<span class="gaia-font">{playerState.income.knowledge}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Hand" />
				<span class="gaia-font">{playerState.income.qic}</span>
			</div>
		</div>
		<div class="row">
			<div class="resource-indicator">
				<ResourceToken type="Power" />
				<span class="gaia-font">{getPowerPoolsSummary(playerState.resources.power)}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Hand" />
				<span class="gaia-font">{`${playerState.income.power} (+${playerState.income.powerTokens})`}</span>
			</div>
		</div>
		<hr />
		<div class="row">
			<div class="resource-indicator">
				<ResourceToken type="Terraformation" scale={0.9} />
				<span class="gaia-font"
					>{`${playerState.terraformingCost}${
						playerState.tempTerraformingSteps ? ` (+${playerState.tempTerraformingSteps})` : ""
					}`}</span
				>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Navigation" scale={0.9} />
				<span class="gaia-font"
					>{`${playerState.navigationRange}${playerState.rangeBoost ? ` (+${playerState.rangeBoost})` : ""}`}</span
				>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="PlanetTypes" scale={0.8} />
				<span class="gaia-font">{playerState.knownPlanetTypes}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken imageUrl="GaiaMarker.png" scale={0.8} />
				<span class="gaia-font">{playerState.gaiaPlanets}</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Sectors" scale={0.7} />
				<span class="gaia-font">{playerState.colonizedSectors}</span>
			</div>
		</div>
		<div class="row-spacer" />
		<div class="row">
			{#if !isNil(player.raceId)}
				<div class="resource-indicator fixed-height">
					<Building raceId={player.raceId} type={BuildingType.Gaiaformer} noAnimation />
					<span class="gaia-font">
						{playerState.usableGaiaformers}/{playerState.unlockedGaiaformers}
					</span>
				</div>
			{/if}
			<div class="resource-indicator fixed-height">
				<ResourceToken type="ActionToken" scale={0.9} />
				<span class="gaia-font">
					{availableSpecialActions}/{allSpecialActions}
				</span>
			</div>
			<div class="resource-indicator">
				<ResourceToken type="Federations" scale={0.9} />
				<span class="gaia-font">{`${playerState.usableFederations}/${playerState.numFederationTokens}`}</span>
			</div>
			<div class="resource-indicator" class:invisible={player.raceId !== Race.Ivits}>
				<ResourceToken type="RecordToken" scale={0.7} />
				<span class="gaia-font">{playerState.additionalInfo}</span>
			</div>
		</div>
		<hr />
		{#if !isNil(player.raceId)}
			<div class="row">
				<div class="resource-indicator fixed-height" class:maxed-out={playerState.buildings.mines === MAX_DEPLOYABLE_MINES}>
					<Building raceId={player.raceId} type={BuildingType.Mine} noAnimation />
					<span class="gaia-font">{playerState.buildings.mines}</span>
					{#if hasLostPlanet}
						<span class="gaia-font">+LP</span>
					{/if}
				</div>
				<div
					class="resource-indicator fixed-height"
					class:maxed-out={playerState.buildings.tradingStations === MAX_DEPLOYABLE_TRADING_STATIONS}
				>
					<Building raceId={player.raceId} type={BuildingType.TradingStation} noAnimation />
					<span class="gaia-font">{playerState.buildings.tradingStations}</span>
				</div>
				<div
					class="resource-indicator fixed-height"
					class:maxed-out={playerState.buildings.researchLabs === MAX_DEPLOYABLE_RESEARCH_LABS}
				>
					<Building raceId={player.raceId} type={BuildingType.ResearchLab} noAnimation />
					<span class="gaia-font">{playerState.buildings.researchLabs}</span>
				</div>
				<div class="resource-indicator fixed-height" class:maxed-out={playerState.buildings.planetaryInstitute}>
					<Building raceId={player.raceId} type={BuildingType.PlanetaryInstitute} noAnimation />
					<span class="gaia-font">{Number(playerState.buildings.planetaryInstitute)}</span>
				</div>
				<div
					class="resource-indicator fixed-height"
					class:maxed-out={playerState.buildings.academyLeft && playerState.buildings.academyRight}
				>
					<Building raceId={player.raceId} type={BuildingType.AcademyLeft} noAnimation />
					<span class="gaia-font">{Number(playerState.buildings.academyLeft) + Number(playerState.buildings.academyRight)}</span>
				</div>
			</div>
		{/if}
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

			> :global(img),
			> :global(.building) {
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
