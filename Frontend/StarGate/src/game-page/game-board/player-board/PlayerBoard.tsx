import _ from "lodash";
import { BrainstoneLocation, BuildingType, Race } from "../../../dto/enums";
import { PlayerInGameDto, PowerPoolsDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { Point } from "../../../utils/miscellanea";
import { getRaceBoard } from "../../../utils/race-utils";
import ActionSpace from "../ActionSpace";
import Building from "../hex/Building";
import ResourceToken from "../ResourceToken";
import useStyles, { buildingGeometries, gfgaSpacing, gfgaX } from "./player-board.styles";

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
	return _.map(_.range(0, qty), () => createPowerTokenAt(poolData.center, poolData.radius, bowl));
}

function createPowerTokenAt({ x, y }: Point, radius: number, bowl: number) {
	const xStretching = bowl === 0 || bowl === 3 ? 2 : 3;
	const yStretching = 1.5;
	const xSign = Math.random() < 0.5 ? -1 : 1;
	const actualX = `${x + xSign * Math.random() * radius * xStretching}%`;
	const ySign = Math.random() < 0.5 ? -1 : 1;
	const actualY = `${y + ySign * Math.random() * radius * yStretching}%`;
	return (
		<div key={`${_.uniqueId(String(bowl))}`} className="powerToken" style={{ top: actualY, left: actualX }}>
			<ResourceToken type="Power" />
		</div>
	);
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
	return (
		<div key={_.uniqueId("Brainstone")} className="powerToken" style={{ top: actualY, left: actualX, zIndex: 1 }}>
			<ResourceToken type="Brainstone" scale={1.5} />
		</div>
	);
}

//#endregion

interface PlayerBoardProps {
	player: PlayerInGameDto;
}

const PlayerBoard = ({ player }: PlayerBoardProps) => {
	const classes = useStyles();
	const imgUrl = useAssetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(player.raceId)}`);
	const isBescods = player?.raceId === Race.Bescods;

	if (_.isNil(player.state)) {
		return <div></div>;
	}

	return (
		<div className={classes.root}>
			<div className={classes.wrapper}>
				<img className={classes.image} src={imgUrl} alt="" />
				<div className={classes.resourceTokens}>
					<div className="token" style={{ left: resourceX(player.state.resources.ores, -1) }}>
						<ResourceToken type="Ores" />
					</div>
					<div className="token" style={{ left: resourceX(player.state.resources.knowledge) }}>
						<ResourceToken type="Knowledge" />
					</div>
					<div className="token" style={{ left: resourceX(Math.min(player.state.resources.credits, 15), 1) }}>
						<ResourceToken type="Credits" />
					</div>
					{player.state.resources.credits > 15 && (
						<div className="token" style={{ left: resourceX(player.state.resources.credits - 15, -2) }}>
							<ResourceToken type="Credits" />
						</div>
					)}
				</div>
				{player.state.raceActionSpace && (
					<div className={`${isBescods ? classes.raceAsBescods : ""} actionSpace`}>
						<ActionSpace space={player.state.raceActionSpace} />
					</div>
				)}
				{!player.state.buildings.planetaryInstitute && (
					<div className={`${isBescods ? classes.piBescods : classes.pi} actionSpace`}>
						<Building type={BuildingType.PlanetaryInstitute} raceId={player.raceId!} />
					</div>
				)}
				{player.state.planetaryInstituteActionSpace && (
					<div className={classes.piAs + " actionSpace"}>
						<ActionSpace space={player.state.planetaryInstituteActionSpace} />
					</div>
				)}
				{!player.state.buildings.academyLeft && (
					<div className={`${isBescods ? classes.aclBescods : classes.acl} actionSpace`}>
						<Building type={BuildingType.AcademyLeft} raceId={player.raceId!} />
					</div>
				)}
				{!player.state.buildings.academyRight && (
					<div className={`${isBescods ? classes.acrBescods : classes.acr} actionSpace`}>
						<Building type={BuildingType.AcademyRight} raceId={player.raceId!} />
					</div>
				)}
				{player.state.rightAcademyActionSpace && (
					<div className={`${isBescods ? classes.acrAsBescods : classes.acrAs} actionSpace`}>
						<ActionSpace space={player.state.rightAcademyActionSpace} />
					</div>
				)}
				{_.map(_.range(0, 4 - player.state.buildings.tradingStations), n => (
					<div key={n} className={classes.ts + " building"} style={{ left: `${(buildingGeometries.ts.x + buildingGeometries.ts.spacing! * (3 - n)) * 100}%` }}>
						<Building type={BuildingType.TradingStation} raceId={player.raceId!} />
					</div>
				))}
				{_.map(_.range(0, 3 - player.state.buildings.researchLabs), n => (
					<div key={n} className={classes.rl + " building"} style={{ left: `${(buildingGeometries.rl.x + buildingGeometries.rl.spacing! * (2 - n)) * 100}%` }}>
						<Building type={BuildingType.ResearchLab} raceId={player.raceId!} />
					</div>
				))}
				{_.map(_.range(0, 8 - player.state.buildings.mines), n => (
					<div key={n} className={classes.mine + " building"} style={{ left: `${(buildingGeometries.mine.x + buildingGeometries.mine.spacing! * (7 - n)) * 100}%` }}>
						<Building type={BuildingType.Mine} raceId={player.raceId!} />
					</div>
				))}
				{_.chain(player.state.availableGaiaformers)
					.filter(gf => gf.available || gf.spentInGaiaArea)
					.map((gf, index) => (
						<div
							key={gf.id}
							className={classes.gf + " building"}
							style={{
								top: gf.available ? `${buildingGeometries.gf.y * 100}%` : `${(gfgaX + gfgaSpacing * (index + 1)) * 100}%`,
								left: gf.available ? `${(buildingGeometries.gf.x + buildingGeometries.gf.spacing! * index) * 100}%` : `${gfgaX * 100}%`,
								height: `${buildingGeometries.gf.h * 80}%`,
							}}
						>
							{<Building type={BuildingType.Gaiaformer} raceId={player.raceId!} />}
						</div>
					))
					.value()}
				{renderPowerTokens(player.state.resources.power, player.raceId!)}
			</div>
		</div>
	);
};

export default PlayerBoard;
