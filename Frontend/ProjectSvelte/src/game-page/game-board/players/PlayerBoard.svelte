<script lang="ts" context="module">
	import { BrainstoneLocation, BuildingType, Race } from "$dto/enums";
	import type { PlayerInGameDto, PowerPoolsDto } from "$dto/interfaces";
	import { Point, withAspectRatioW } from "$utils/miscellanea";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceBoard } from "$utils/race-utils";
	import { range } from "lodash";
	import ActionSpace from "../ActionSpace.svelte";
	import Building from "../map/hex/Building.svelte";
	import ResourceToken from "../ResourceToken.svelte";

	const boardVersion = "rework";

	//#region Resources

	const resourceGeometry = new Map<number, { base: number; multiplier: number }>([
		[0, { base: 5, multiplier: 1 }],
		[1, { base: 13, multiplier: 0.5 }],
		[2, { base: 19, multiplier: 0.5 }],
		[3, { base: 25, multiplier: 0.5 }],
		[4, { base: 32, multiplier: 0.5 }],
		[5, { base: 38, multiplier: 0.5 }],
		[6, { base: 44, multiplier: 0.5 }],
		[7, { base: 50.5, multiplier: 0.5 }],
		[8, { base: 56.5, multiplier: 0.5 }],
		[9, { base: 62.5, multiplier: 0.5 }],
		[10, { base: 69.5, multiplier: 0.5 }],
		[11, { base: 75, multiplier: 0.5 }],
		[12, { base: 80, multiplier: 0.5 }],
		[13, { base: 85, multiplier: 0.5 }],
		[14, { base: 89.5, multiplier: 0.5 }],
		[15, { base: 94.5, multiplier: 0.5 }],
	]);
	const resourceX = (qty: number, offset = 0) => {
		const { base, multiplier } = resourceGeometry.get(qty) ?? { base: 0, multiplier: 0 };
		return `${base + offset * multiplier}%`;
	};

	const powerPoolsGeometry = [
		{ center: { x: 9, y: 21 }, radius: 2.5 },
		{ center: { x: 24, y: 33 }, radius: 2.5 },
		{ center: { x: 24, y: 15 }, radius: 2.5 },
		{ center: { x: 45.5, y: 22.5 }, radius: 2.5 },
	];

	function renderPowerTokens(powerPools: PowerPoolsDto, race: Race | null) {
		const bowl1Tokens = createPowerTokens(1, powerPools.bowl1);
		const bowl2Tokens = createPowerTokens(2, powerPools.bowl2);
		const bowl3Tokens = createPowerTokens(3, powerPools.bowl3);
		const gaiaAreaTokens = createPowerTokens(0, powerPools.gaiaArea);
		let brainstone = null;
		if (race === Race.Taklons && powerPools.brainstone! > BrainstoneLocation.Removed) {
			brainstone = createBrainstone(powerPools.brainstone!);
		}

		return [...bowl1Tokens, ...bowl2Tokens, ...bowl3Tokens, ...gaiaAreaTokens, ...(brainstone ? [brainstone] : [])];
	}

	// Returns object instances for each token created
	function createPowerTokens(bowl: number, qty: number) {
		const poolData = powerPoolsGeometry[bowl];
		return range(0, qty).map(() => createPowerTokenAt(poolData.center, poolData.radius, bowl));
	}

	function createPowerTokenAt({ x, y }: Point, radius: number, bowl: number) {
		// const xStretching = bowl === 0 || bowl === 3 ? 2 : 3;
		// const yStretching = 1.5;
		// const xSign = Math.random() < 0.5 ? -1 : 1;
		// const actualX = `${x + xSign * Math.random() * radius * xStretching}%`;
		// const ySign = Math.random() < 0.5 ? -1 : 1;
		// const actualY = `${y + ySign * Math.random() * radius * yStretching}%`;
		// return (
		// 	<div key={`${uniqueId(String(bowl))}`} class="powerToken" style={{ top: actualY, left: actualX }}>
		// 		<ResourceToken type="Power" />
		// 	</div>
		// );
	}

	function createBrainstone(location: BrainstoneLocation) {
		const actualBowl = location === BrainstoneLocation.GaiaArea ? 0 : location;
		const poolData = powerPoolsGeometry[actualBowl];
		const { x, y } = poolData.center;
		const radius = poolData.radius;
		const xStretching = location === 4 || location === 3 ? 2 : 3;
		const xSign = location === 3 ? 1 : -1;
		const actualX = `${x + xSign * radius * xStretching}%`;
		const actualY = `${y}%`;
		// return (
		// 	<div key={uniqueId("Brainstone")} class="powerToken" style={{ top: actualY, left: actualX, zIndex: 1 }}>
		// 		<ResourceToken type="Brainstone" scale={1.5} />
		// 	</div>
		// );
	}

	//#endregion

	const WIDTH_TO_HEIGHT_RATIO = 1.577;

	//#region Items geometry

	interface BuildingGeometry {
		w: number;
		h: number;
		x: number;
		y: number;
		spacing?: number;
	}

	const resourcesH = 0.055;
	const buildingGeometries: { [type: string]: BuildingGeometry } = {
		mine: {
			w: 0.0381,
			h: 0.085,
			x: 0.1315,
			y: 0.865,
			spacing: 0.0502,
		},
		ts: {
			w: 0.0381,
			h: 0.0984,
			x: 0.1315,
			y: 0.68,
			spacing: 0.0502,
		},
		rl: {
			w: 0.0623,
			h: 0.0984,
			x: 0.4758,
			y: 0.68,
			spacing: 0.0727,
		},
		pi: {
			w: 0.083,
			h: 0.1311,
			x: 0.1384,
			y: 0.459,
		},
		piBescods: {
			w: 0.083,
			h: 0.1311,
			x: 0.45,
			y: 0.459,
		},
		piAsAmbas: {
			w: 0.08,
			h: 0.098,
			x: 0.235,
			y: 0.4875,
		},
		piAsFiraks: {
			w: 0.08,
			h: 0.098,
			x: 0.2075,
			y: 0.49,
		},
		piAsIvits: {
			w: 0.07,
			h: 0.098,
			x: 0.2375,
			y: 0.48,
		},
		acl: {
			w: 0.0692,
			h: 0.153,
			x: 0.4814,
			y: 0.4481,
		},
		aclBescods: {
			w: 0.0692,
			h: 0.153,
			x: 0.11,
			y: 0.4481,
		},
		acr: {
			w: 0.0692,
			h: 0.153,
			x: 0.5955,
			y: 0.4481,
		},
		acrBescods: {
			w: 0.0692,
			h: 0.153,
			x: 0.22,
			y: 0.4481,
		},
		acrAs: {
			w: 0.06,
			h: 0.098,
			x: 0.61,
			y: 0.5,
		},
		acrAsBescods: {
			w: 0.06,
			h: 0.098,
			x: 0.24,
			y: 0.5,
		},
		gf: {
			w: 0.0623,
			h: 0.0984,
			x: 0.75,
			y: 0.305,
			spacing: 0.0775,
		},
		raceAsBescods: {
			w: 0.07,
			h: 0.098,
			x: 0.885,
			y: 0.1675,
		},
	};
	const gfgaX = 0.01;
	const gfgaY = 0.075;
	const gfgaSpacing = 0.085;

	//#endregion

	const boardElementStyle = (type: string, left?: number) => `
		width: ${buildingGeometries[type].w * 100}%;
		height: ${buildingGeometries[type].h * 100}%;
		top: ${buildingGeometries[type].y * 100}%;
		left: ${left ?? buildingGeometries[type].x * 100}%;
	`;
</script>

<script lang="ts">
	export let player: PlayerInGameDto;

	$: isBescods = player?.raceId === Race.Bescods;
	$: race = player?.raceId ?? Race.None;
</script>

{#if player?.state}
	<div style={withAspectRatioW(WIDTH_TO_HEIGHT_RATIO)}>
		<div class="wh-full absolute top-0 left-0 flex justify-center items-center">
			<img class="wh-full" src={assetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(player.raceId)}`)} alt="" />
			<div class="resource-tokens absolute top-0 left-0 w-full" style:height={`${resourcesH * 100}%`}>
				<div class="token" style:left={resourceX(player.state.resources.ores, -1)}>
					<ResourceToken type="Ores" />
				</div>
				<div class="token" style:left={resourceX(player.state.resources.knowledge)}>
					<ResourceToken type="Knowledge" />
				</div>
				<div class="token" style:left={resourceX(Math.min(player.state.resources.credits, 15), 1)}>
					<ResourceToken type="Credits" />
				</div>
				{#if player.state.resources.credits > 15}
					<div class="token" style:left={resourceX(player.state.resources.credits - 15, -2)}>
						<ResourceToken type="Credits" />
					</div>
				{/if}
			</div>
			{#if player.state.raceActionSpace}
				<div class="action-space" style={isBescods ? boardElementStyle("raceAsBescods") : ""}>
					<ActionSpace space={player.state.raceActionSpace} />
				</div>
			{/if}
			{#if !player.state.buildings.planetaryInstitute}
				<div class="action-space" style={isBescods ? boardElementStyle("piBescods") : boardElementStyle("pi")}>
					<Building type={BuildingType.PlanetaryInstitute} raceId={race} />
				</div>
			{/if}
			{#if player.state.planetaryInstituteActionSpace}
				<div
					class="action-space"
					style={boardElementStyle(race === Race.Ambas ? "piAsAmbas" : race === Race.Firaks ? "piAsFiraks" : "piAsIvits")}
				>
					<ActionSpace space={player.state.planetaryInstituteActionSpace} />
				</div>
			{/if}
			{#if !player.state.buildings.academyLeft}
				<div class="action-space" style={isBescods ? boardElementStyle("aclBescods") : boardElementStyle("acl")}>
					<Building type={BuildingType.AcademyLeft} raceId={race} />
				</div>
			{/if}
			{#if !player.state.buildings.academyRight}
				<div class="action-space" style={isBescods ? boardElementStyle("acrBescods") : boardElementStyle("acr")}>
					<Building type={BuildingType.AcademyRight} raceId={race} />
				</div>
			{/if}
			{#if player.state.rightAcademyActionSpace}
				<div class="action-space" style={isBescods ? boardElementStyle("acrAsBescods") : boardElementStyle("acrAs")}>
					<ActionSpace space={player.state.rightAcademyActionSpace} />
				</div>
			{/if}
			{#each range(0, 4 - player.state.buildings.tradingStations) as n}
				<div
					class="building-wrapper"
					style={`${boardElementStyle("ts", (buildingGeometries.ts.x + (buildingGeometries.ts.spacing || 0) * (3 - n)) * 100)}`}
				>
					<Building type={BuildingType.TradingStation} raceId={race} />
				</div>
			{/each}
			{#each range(0, 3 - player.state.buildings.researchLabs) as n}
				<div
					class="building-wrapper"
					style={`${boardElementStyle("rl", (buildingGeometries.rl.x + (buildingGeometries.rl.spacing || 0) * (2 - n)) * 100)}`}
				>
					<Building type={BuildingType.ResearchLab} raceId={race} />
				</div>
			{/each}
			{#each range(0, 8 - player.state.buildings.mines) as n}
				<div
					class="building-wrapper"
					style={`${boardElementStyle(
						"mine",
						(buildingGeometries.mine.x + (buildingGeometries.mine.spacing || 0) * (7 - n)) * 100
					)}`}
				>
					<Building type={BuildingType.Mine} raceId={race} />
				</div>
			{/each}
			{#each player.state.availableGaiaformers.filter(gf => gf.available || gf.spentInGaiaArea) as gf, index (gf.id)}
				<div
					class="building-wrapper"
					style={`
						top: ${(gf.available ? buildingGeometries.gf.y : gfgaX + gfgaSpacing * (index + 1)) * 100}%;
						left: ${(gf.available ? buildingGeometries.gf.x + (buildingGeometries.gf.spacing || 0) * index : gfgaX) * 100}%;
						height: ${buildingGeometries.gf.h * 80}%
					`}
				>
					<Building type={BuildingType.Gaiaformer} raceId={race} />
				</div>
			{/each}
			<!-- {renderPowerTokens(player.state.resources.power, race)} -->
		</div>
	</div>
{/if}

<style>
	.token {
		position: absolute;
		top: 15%;
		width: 3%;
	}

	.action-space {
		position: absolute;
	}

	.building-wrapper {
		position: absolute;
	}

	.power-token {
		position: absolute;
		width: 3%;
	}
</style>
