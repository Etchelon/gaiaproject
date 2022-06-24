<script lang="ts" context="module">
	import { BrainstoneLocation, Conversion, Race } from "$dto/enums";
	import type { PlayerInGameDto, PlayerStateDto, ResourcesDto } from "$dto/interfaces";
	import { chain, cloneDeep } from "lodash";

	const BRAINSTONE_POWER_VALUE = 3;
	const MAX_ORES_KNOWLEDGE = 15;
	const MAX_CREDITS = 30;

	const sizeAndPosition = (width: number, height: number, top: number, left: number) => `
		width: ${width}%;
		height: ${height}%;
		top: ${top}%;
		left: ${left}%;
	`;

	const ores = (playerState: PlayerStateDto) => playerState.resources.ores;
	const credits = (playerState: PlayerStateDto) => playerState.resources.credits;
	const knowledge = (playerState: PlayerStateDto) => playerState.resources.knowledge;
	const qic = (playerState: PlayerStateDto) => playerState.resources.qic;
	const brainstone = (playerState: PlayerStateDto) => playerState.resources.power.brainstone;
	const power1 = (playerState: PlayerStateDto) => playerState.resources.power.bowl1;
	const power2 = (playerState: PlayerStateDto) => playerState.resources.power.bowl2;
	const actualPower2 = (playerState: PlayerStateDto) =>
		power2(playerState) + Number(brainstone(playerState) === BrainstoneLocation.Bowl2);
	const power3 = (playerState: PlayerStateDto) => playerState.resources.power.bowl3;
	const actualPower3 = (playerState: PlayerStateDto) =>
		power3(playerState) + BRAINSTONE_POWER_VALUE * Number(brainstone(playerState) === BrainstoneLocation.Bowl3);
	const powerGaia = (playerState: PlayerStateDto) => playerState.resources.power.gaiaArea;
	const availableGaiaformersCount = (playerState: PlayerStateDto) =>
		chain(playerState.availableGaiaformers)
			.filter(gf => gf.available)
			.size()
			.value();
	const range = (playerState: PlayerStateDto) => playerState.navigationRange;
	const rangeBoost = (playerState: PlayerStateDto) => playerState.rangeBoost;

	const canBurn = (playerState: PlayerStateDto) => actualPower2(playerState) >= 2;
	const hasPlanetaryInstitute = (playerState: PlayerStateDto) => playerState.buildings.planetaryInstitute;
	const hasQicAcademy = (playerState: PlayerStateDto) => playerState.buildings.academyRight;
	const playerIs = (p: PlayerInGameDto, race: Race) => p.raceId === race;
	const isHadschHallasWithPlanetaryInstitute = (player: PlayerInGameDto) =>
		playerIs(player, Race.HadschHallas) && hasPlanetaryInstitute(player.state);
	const isNevlas = (player: PlayerInGameDto) => playerIs(player, Race.Nevlas);
	const isNevlasWithPlanetaryInstitute = (player: PlayerInGameDto) => isNevlas(player) && hasPlanetaryInstitute(player.state);
	const isGleensWithoutQicAcademy = (player: PlayerInGameDto) => playerIs(player, Race.Gleens) && !hasQicAcademy(player.state);
	const isTaklons = (player: PlayerInGameDto) => playerIs(player, Race.Taklons);
	const isTaklonsWithBrainstone = (player: PlayerInGameDto) => {
		if (!isTaklons(player)) {
			return false;
		}
		const brainstone_ = brainstone(player.state);
		return brainstone_! !== BrainstoneLocation.GaiaArea && brainstone_! !== BrainstoneLocation.Removed;
	};

	const equivalentPower3 = (player: PlayerInGameDto) => actualPower3(player.state) * (isNevlasWithPlanetaryInstitute(player) ? 2 : 1);

	interface PlayerWithConversions {
		player: PlayerInGameDto;
		conversions: Conversion[];
	}

	interface ActionType {
		type: Conversion | "reset";
		data?: any;
	}

	const reducer = (state: PlayerWithConversions, action: ActionType): PlayerWithConversions => {
		const newState = cloneDeep(state);
		const player = newState.player;
		const playerState = newState.player.state;
		const resources = newState.player.state.resources;
		const power = newState.player.state.resources.power;

		const spendPower = (amount: number) => {
			if (
				isTaklonsWithBrainstone(player) &&
				brainstone(playerState) === BrainstoneLocation.Bowl3 &&
				amount >= BRAINSTONE_POWER_VALUE
			) {
				const excess = amount - BRAINSTONE_POWER_VALUE;
				power.bowl3 -= excess;
				power.bowl1 += excess;
				power.brainstone = BrainstoneLocation.Bowl1;
			} else {
				power.bowl3 -= amount;
				power.bowl1 += amount;
			}
		};

		switch (action.type) {
			case "reset":
				return { player: cloneDeep(action.data! as PlayerInGameDto), conversions: [] };
			case Conversion.BurnPower:
				if (isTaklonsWithBrainstone(player) && brainstone(playerState) === BrainstoneLocation.Bowl2) {
					power.bowl2 -= 1;
					power.brainstone = BrainstoneLocation.Bowl3;
				} else {
					power.bowl2 -= 2;
					power.bowl3 += 1;
				}

				if (playerIs(player, Race.Itars)) {
					power.gaiaArea += 1;
				}
				break;
			case Conversion.BoostRange:
				resources.qic -= 1;
				playerState.rangeBoost = (playerState.rangeBoost ?? 0) + 2;
				break;
			case Conversion.QicToOre:
				resources.qic -= 1;
				resources.ores += 1;
				break;
			case Conversion.PowerToQic:
				resources.qic += 1;
				if (isNevlasWithPlanetaryInstitute(player)) {
					spendPower(2);
					newState.conversions.push(Conversion.Nevlas2PowerToQic);
					return newState;
				}
				spendPower(4);
				break;
			case Conversion.PowerToOre:
				resources.ores += 1;
				if (isNevlasWithPlanetaryInstitute(player)) {
					spendPower(2);
					resources.credits += 1;
					newState.conversions.push(Conversion.Nevlas2PowerToOreAndCredit);
					return newState;
				}
				spendPower(3);
				break;
			case Conversion.PowerToKnowledge:
				resources.knowledge += 1;
				if (isNevlasWithPlanetaryInstitute(player)) {
					spendPower(2);
					newState.conversions.push(Conversion.Nevlas2PowerToKnowledge);
					return newState;
				}
				spendPower(4);
				break;
			case Conversion.KnowledgeToCredit:
				resources.knowledge -= 1;
				resources.credits += 1;
				break;
			case Conversion.PowerToCredit:
				spendPower(1);
				if (isNevlasWithPlanetaryInstitute(player)) {
					resources.credits += 2;
					newState.conversions.push(Conversion.NevlasPowerTo2Credits);
					return newState;
				}
				resources.credits += 1;
				break;
			case Conversion.OreToCredit:
				resources.ores -= 1;
				resources.credits += 1;
				break;
			case Conversion.OreToPowerToken:
				resources.ores -= 1;
				if (isTaklons(player) && brainstone(playerState) === BrainstoneLocation.Removed) {
					power.brainstone = BrainstoneLocation.Bowl1;
				} else {
					power.bowl1 += 1;
				}
				break;
			case Conversion.NevlasPower3ToKnowledge:
				power.bowl3 -= 1;
				power.gaiaArea += 1;
				resources.knowledge += 1;
				break;
			case Conversion.Nevlas3PowerTo2Ores:
				power.bowl3 -= 3;
				power.bowl1 += 3;
				resources.ores += 2;
				break;
			case Conversion.HadschHallas4CreditsToQic:
				resources.credits -= 4;
				resources.qic += 1;
				break;
			case Conversion.HadschHallas4CreditsToKnowledge:
				resources.credits -= 4;
				resources.knowledge += 1;
				break;
			case Conversion.HadschHallas3CreditsToOre:
				resources.credits -= 3;
				resources.ores += 1;
				break;
			case Conversion.BalTaksGaiaformerToQic:
				const gfToSpend = chain(playerState.availableGaiaformers)
					.filter(gf => gf.available)
					.first()
					.value();
				gfToSpend.available = false;
				gfToSpend.spentInGaiaArea = true;
				resources.qic += 1;
				break;
			case Conversion.TaklonsBrainstoneToCredits:
				spendPower(BRAINSTONE_POWER_VALUE);
				resources.credits += 3;
				break;
			default:
				throw new Error(`Action ${action} not implemented.`);
		}
		newState.conversions.push(action.type);
		return newState;
	};
</script>

<script lang="ts">
	import ClickableRectangle from "$components/ClickableRectangle.svelte";
	import { assetUrl } from "$utils/miscellanea";
	import { isEmpty } from "lodash";
	import { get, writable } from "svelte/store";
	import ResourceToken from "../../game-board/ResourceToken.svelte";
	import { getGamePageContext } from "../../GamePage.context";
	import { PassTurn, PerformConversionsOrPassTurnWorkflow } from "../../workflows/rounds-phase/perform-conversions-or-pass.workflow";
	import { CommonWorkflowStates } from "../../workflows/types";

	export let gameId: string;
	export let currentPlayer: PlayerInGameDto;

	let isPerformingConversion = false;
	const { store, activeWorkflow: aw } = getGamePageContext();
	const activeWorkflow = get(aw);
	const isFinalConversions = activeWorkflow instanceof PerformConversionsOrPassTurnWorkflow;
	const conversionsState = writable<PlayerWithConversions>({
		player: cloneDeep(currentPlayer),
		conversions: [],
	});
	const dispatch = (action: ActionType) => {
		conversionsState.update(state => reducer(state, action));
	};

	const reset = () => {
		dispatch({ type: "reset", data: currentPlayer });
	};

	const cancel = (pass: boolean) => () => {
		const nextState = isFinalConversions && pass ? PassTurn : CommonWorkflowStates.CANCEL;
		const action = activeWorkflow?.handleCommand({ nextState });
		action && store.executePlayerAction(gameId, action);
	};

	const convert = () => {
		const action = activeWorkflow?.handleCommand({
			nextState: CommonWorkflowStates.PERFORM_CONVERSION,
			data: $conversionsState.conversions,
		})!;
		store.executePlayerAction(gameId, action);
		isPerformingConversion = true;
	};

	$: player = $conversionsState.player;
	$: playerState = player.state;
</script>

<ion-header>
	<ion-toolbar>
		<ion-title class="gaia-font">Conversions</ion-title>
	</ion-toolbar>
</ion-header>
<ion-content fullscreen>
	<div class="grid grid-cols-4 gap-2 p-1 md:p-3">
		<div class="col-span-2 md:col-span-1">
			<div class="relative">
				<img class="w-full object-contain" src={assetUrl("Conversions.png")} alt="" />
				<ClickableRectangle
					style={sizeAndPosition(14, 6.35, 15, 53)}
					active={qic(playerState) > 0}
					on:click={() => dispatch({ type: Conversion.BoostRange })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(14, 6.35, 30, 53)}
					active={qic(playerState) > 0}
					on:click={() => dispatch({ type: Conversion.QicToOre })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(14, 6.35, 55, 53)}
					active={knowledge(playerState) > 0}
					on:click={() => dispatch({ type: Conversion.KnowledgeToCredit })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 23, 34)}
					active={equivalentPower3(player) >= 4 && !isGleensWithoutQicAcademy(player)}
					on:click={() => dispatch({ type: Conversion.PowerToQic })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 37.5, 34)}
					active={equivalentPower3(player) >= 3}
					on:click={() => dispatch({ type: Conversion.PowerToOre })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 48, 34)}
					active={equivalentPower3(player) >= 4}
					on:click={() => dispatch({ type: Conversion.PowerToKnowledge })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 62, 34)}
					active={equivalentPower3(player) > 0 &&
						!(equivalentPower3(player) === 3 && brainstone(playerState) === BrainstoneLocation.Bowl3)}
					on:click={() => dispatch({ type: Conversion.PowerToCredit })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 70.5, 34)}
					active={ores(playerState) > 0}
					on:click={() => dispatch({ type: Conversion.OreToCredit })}
				/>
				<ClickableRectangle
					style={sizeAndPosition(16, 5.9, 84.5, 34)}
					active={ores(playerState) > 0}
					on:click={() => dispatch({ type: Conversion.OreToPowerToken })}
				/>
			</div>
		</div>
		<div class="col-span-2 md:col-span-1 overflow-x-hidden overflow-y-auto">
			<p class="gaia-font text-center">Other</p>
			<ion-button class="w-full mt-1 py-1 px-2" disabled={!canBurn} on:click={() => dispatch({ type: Conversion.BurnPower })}>
				<span class="gaia-font">Burn 1 Power</span>
			</ion-button>
			{#if isNevlas(player)}
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={power3(playerState) <= 0}
					on:click={() => dispatch({ type: Conversion.NevlasPower3ToKnowledge })}
				>
					<span class="gaia-font">{"Bowl 3 -> Knowledge"}</span>
				</ion-button>
			{/if}
			{#if isNevlasWithPlanetaryInstitute(player)}
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={equivalentPower3(player) < 6}
					on:click={() => dispatch({ type: Conversion.Nevlas3PowerTo2Ores })}
				>
					<span class="gaia-font">{"3 Power -> 2 Ores"}</span>
				</ion-button>
			{/if}
			{#if isHadschHallasWithPlanetaryInstitute(player)}
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={credits(playerState) < 4}
					on:click={() => dispatch({ type: Conversion.HadschHallas4CreditsToQic })}
				>
					<span class="gaia-font">{"4 Credits -> Qic"}</span>
				</ion-button>
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={credits(playerState) < 4}
					on:click={() => dispatch({ type: Conversion.HadschHallas4CreditsToKnowledge })}
				>
					<span class="gaia-font">{"4 Credits -> Knowledge"}</span>
				</ion-button>
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={credits(playerState) < 3}
					on:click={() => dispatch({ type: Conversion.HadschHallas3CreditsToOre })}
				>
					<span class="gaia-font">{"3 Credits -> Ore"}</span>
				</ion-button>
			{/if}
			{#if playerIs(player, Race.BalTaks)}
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={availableGaiaformersCount(playerState) === 0}
					on:click={() => dispatch({ type: Conversion.BalTaksGaiaformerToQic })}
				>
					<span class="gaia-font">{"Gaiaformer -> Qic"}</span>
				</ion-button>
			{/if}
			{#if playerIs(player, Race.Taklons)}
				<ion-button
					class="w-full mt-1 py-1 px-2"
					disabled={brainstone(playerState) !== BrainstoneLocation.Bowl3}
					on:click={() => dispatch({ type: Conversion.TaklonsBrainstoneToCredits })}
				>
					<span class="gaia-font">{"Brainstone -> 3 Credits"}</span>
				</ion-button>
			{/if}
		</div>
		<div class="col-span-4 md:col-span-2">
			<div class="flex flex-col items-stretch h-full">
				<div class="player-data">
					<h6 class="gaia-font text-center">Resources</h6>
					<div class="row">
						<div class="resource-indicator" class:maxed-out={ores(playerState) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Ores" />
							<span class="gaia-font">{ores(playerState)}</span>
						</div>
						<div class="resource-indicator" class:maxed-out={credits(playerState) === MAX_CREDITS}>
							<ResourceToken type="Credits" />
							<span class="gaia-font">{credits(playerState)}</span>
						</div>
						<div class="resource-indicator" class:maxed-out={knowledge(playerState) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Knowledge" />
							<span class="gaia-font">{knowledge(playerState)}</span>
						</div>
						<div class="resource-indicator">
							<ResourceToken type="Qic" />
							<span class="gaia-font">{qic(playerState)}</span>
						</div>
					</div>
					<h6 class="gaia-font text-center">Power Bowls</h6>
					<div class="row">
						<div class="resource-indicator" class:maxed-out={ores(playerState) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Power" />
							<span class="gaia-font"
								>{`${power1(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span
							>
						</div>
						<div class="resource-indicator" class:maxed-out={credits(playerState) === MAX_CREDITS}>
							<ResourceToken type="Power" />
							<span class="gaia-font"
								>{`${power2(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span
							>
						</div>
						<div class="resource-indicator" class:maxed-out={knowledge(playerState) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Power" />
							<span class="gaia-font"
								>{`${power3(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span
							>
						</div>
						<div class="resource-indicator">
							<ResourceToken type="Power" />
							<span class="gaia-font"
								>{`${powerGaia(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span
							>
						</div>
					</div>
					<h6 class="gaia-font text-center">Skills</h6>
					<div class="row justify-around">
						<div class="resource-indicator" class:maxed-out={ores(playerState) === MAX_ORES_KNOWLEDGE}>
							<ResourceToken type="Navigation" />
							<span class="gaia-font"
								>{`${range(playerState)}${rangeBoost(playerState) ? ` +(${rangeBoost(playerState)})` : ""}`}</span
							>
						</div>
						<div class="resource-indicator" class:maxed-out={credits(playerState) === MAX_CREDITS}>
							<ResourceToken type="Gaiaformation" scale={2} />
							<span class="gaia-font">{availableGaiaformersCount(playerState)}</span>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</ion-content>
<ion-footer>
	<ion-toolbar>
		<ion-buttons slot="end">
			{#if isFinalConversions}
				<ion-button size="small" on:click={cancel(true)}>
					<span class="gaia-font">Pass</span>
				</ion-button>
			{/if}
			<ion-button size="small" on:click={cancel(false)}>
				<span class="gaia-font">Cancel</span>
			</ion-button>
			<ion-button size="small" color="warning" on:click={reset}>
				<span class="gaia-font">Reset</span>
			</ion-button>
			<ion-button
				size="small"
				color="primary"
				disabled={isPerformingConversion || isEmpty($conversionsState.conversions)}
				on:click={convert}
			>
				<span class="gaia-font">Confirm</span>
			</ion-button>
		</ion-buttons>
	</ion-toolbar>
</ion-footer>

<style lang="scss">
	$spacingHalf: 4px;
	$spacing1: 8px;

	.player-data {
		padding: $spacingHalf $spacing1;
		background-color: white;
		color: black;

		.row {
			display: flex;
			align-items: center;
			justify-content: space-between;
			height: 30px;
		}

		.resource-indicator {
			display: flex;
			align-items: center;

			> :global(img),
			> :global(.building) {
				width: 35px;
				margin-right: $spacingHalf;
			}

			> span {
				font-size: 0.9rem;
			}
		}

		.maxed-out > span {
			color: rgb(210, 19, 24);
		}
	}
</style>
