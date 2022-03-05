import { useTheme } from "@mui/material";
import Typography from "@mui/material/Typography";
import { isNil, memoize } from "lodash";
import { observer } from "mobx-react";
import VpImg from "../../../assets/Resources/Markers/VP.png";
import ActivePlayerImg from "../../../assets/Resources/PlayerLoader.gif";
import { BuildingType, Race } from "../../../dto/enums";
import { PlayerInGameDto, PowerPoolsDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { countActivatableActions } from "../../../utils/miscellanea";
import { getRaceColor, getRaceImage, getRaceName } from "../../../utils/race-utils";
import { useGamePageContext } from "../../GamePage.context";
import { selectIsOnline } from "../../store/selectors";
import Building from "../hex/Building";
import ResourceToken from "../ResourceToken";
import styles from "./PlayerBox.module.scss";

const MAX_ORES_KNOWLEDGE = 15;
const MAX_CREDITS = 30;
const MAX_DEPLOYABLE_MINES = 8;
const MAX_DEPLOYABLE_TRADING_STATIONS = 4;
const MAX_DEPLOYABLE_RESEARCH_LABS = 3;
const MAX_RESEARCH_STEPS = 5;

interface PlayerBoxProps {
	player: PlayerInGameDto;
	index: number;
}

const getPowerPoolsSummary = memoize((powerPools: PowerPoolsDto): string => {
	const bowl1 = powerPools.bowl1 + (powerPools.brainstone === 1 ? " (B)" : "");
	const bowl2 = powerPools.bowl2 + (powerPools.brainstone === 2 ? " (B)" : "");
	const bowl3 = powerPools.bowl3 + (powerPools.brainstone === 3 ? " (B)" : "");
	const gaia = `Gaia: ${powerPools.gaiaArea}` + (powerPools.brainstone === 4 ? " (B)" : "");
	return `${bowl1}/${bowl2}/${bowl3} (${gaia})`;
});

const PlayerBox = ({ player, index }: PlayerBoxProps) => {
	const theme = useTheme();
	const imgUrl = useAssetUrl(`Races/${getRaceImage(player.raceId)}`);
	const { vm } = useGamePageContext();
	const isInitialized = !isNil(player.raceId) && !isNil(player.state);
	const isOnline = selectIsOnline(player.id)(vm);

	if (!isInitialized) {
		return (
			<div className={styles.playerBox} style={{ backgroundColor: "white" }}>
				<div className={styles.header} style={{ color: "black" }}>
					<img className={styles.avatar} src={ActivePlayerImg} alt="" style={{ opacity: player.isActive ? 1 : 0 }} />
					<div className={styles.info}>
						<Typography variant="body1" className={styles.username + " gaia-font ellipsify"}>
							{player.username}
						</Typography>
						<div className={styles.raceName}>
							<div className={styles.connectionStatus} style={{ backgroundColor: isOnline ? "green" : "grey" }}></div>
							<Typography variant="caption" className="gaia-font">
								No race selected
							</Typography>
						</div>
					</div>
					<div className={styles.stats}>
						<Typography variant="body1" className={styles.order + " gaia-font"}>
							{`${index}°`}
						</Typography>
						<div className={styles.points}>
							<span className="gaia-font">-</span>
							<img className={styles.vpImg} src={VpImg} alt="" />
						</div>
					</div>
				</div>
			</div>
		);
	}

	const color = getRaceColor(player.raceId);
	const textColor = theme.palette.getContrastText(color);
	const { available: availableSpecialActions, all: allSpecialActions } = countActivatableActions(player, false);
	const playerState = player.state!;
	const hasLostPlanet = playerState.researchAdvancements.navigation === MAX_RESEARCH_STEPS;
	const showNextRoundTurnOrder = playerState.hasPassed && playerState.nextRoundTurnOrder !== null;

	return (
		<div className={`${styles.playerBox}${player.state.hasPassed ? ` ${styles.hasPassed}` : ""}`} style={{ backgroundColor: color }}>
			<div className={styles.header} style={{ color: textColor }}>
				<img className={styles.avatar} src={player.isActive ? ActivePlayerImg : imgUrl} alt="" />
				<div className={styles.info}>
					<Typography variant="body1" className={styles.username + " gaia-font ellipsify"}>
						{player.username}
					</Typography>
					<div className={styles.raceName}>
						<div className={styles.connectionStatus} style={{ backgroundColor: isOnline ? "green" : "grey" }}></div>
						<Typography variant="caption" className="gaia-font">
							{getRaceName(player.raceId ?? 0)}
						</Typography>
					</div>
				</div>
				<div className={styles.stats}>
					<Typography variant="body1" className={styles.order + " gaia-font"}>
						{showNextRoundTurnOrder ? `N: ${playerState.nextRoundTurnOrder}°` : `${playerState.currentRoundTurnOrder}°`}
					</Typography>
					<div className={styles.points}>
						<span className="gaia-font">{playerState.points + ((playerState.auctionPoints ?? 0) > 0 ? ` (-${playerState.auctionPoints})` : "")}</span>
						<img className={styles.vpImg} src={VpImg} alt="" />
					</div>
				</div>
			</div>
			<div className={styles.playerData}>
				<div className={styles.row}>
					<div className={`${styles.resourceIndicator} ${playerState.resources.ores === MAX_ORES_KNOWLEDGE ? styles.maxedOut : ""}`}>
						<ResourceToken type="Ores" />
						<span className="gaia-font">{playerState.resources.ores}</span>
					</div>
					<div className={`${styles.resourceIndicator} ${playerState.resources.credits === MAX_CREDITS ? styles.maxedOut : ""}`}>
						<ResourceToken type="Credits" />
						<span className="gaia-font">{playerState.resources.credits}</span>
					</div>
					<div className={`${styles.resourceIndicator} ${playerState.resources.knowledge === MAX_ORES_KNOWLEDGE ? styles.maxedOut : ""}`}>
						<ResourceToken type="Knowledge" />
						<span className="gaia-font">{playerState.resources.knowledge}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Qic" />
						<span className="gaia-font">{playerState.resources.qic}</span>
					</div>
				</div>
				<div className={styles.row}>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Hand" />
						<span className="gaia-font">{playerState.income.ores}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Hand" />
						<span className="gaia-font">{playerState.income.credits}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Hand" />
						<span className="gaia-font">{playerState.income.knowledge}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Hand" />
						<span className="gaia-font">{playerState.income.qic}</span>
					</div>
				</div>
				<div className={styles.row}>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Power" />
						<span className="gaia-font">{getPowerPoolsSummary(playerState.resources.power)}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Hand" />
						<span className="gaia-font">{`${playerState.income.power} (+${playerState.income.powerTokens})`}</span>
					</div>
				</div>
				<hr />
				<div className={styles.row}>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Terraformation" scale={0.9} />
						<span className="gaia-font">{`${playerState.terraformingCost}${playerState.tempTerraformingSteps ? ` (+${playerState.tempTerraformingSteps})` : ""}`}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Navigation" scale={0.9} />
						<span className="gaia-font">{`${playerState.navigationRange}${playerState.rangeBoost ? ` (+${playerState.rangeBoost})` : ""}`}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="PlanetTypes" scale={0.8} />
						<span className="gaia-font">{playerState.knownPlanetTypes}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken assetUrl="GaiaMarker.png" scale={0.8} />
						<span className="gaia-font">{playerState.gaiaPlanets}</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Sectors" scale={0.7} />
						<span className="gaia-font">{playerState.colonizedSectors}</span>
					</div>
				</div>
				<div className={styles.rowSpacer}></div>
				<div className={styles.row}>
					{player.raceId !== null && (
						<div className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight}`}>
							<Building raceId={player.raceId} type={BuildingType.Gaiaformer} noAnimation />
							<span className="gaia-font">
								{playerState.usableGaiaformers}/{playerState.unlockedGaiaformers}
							</span>
						</div>
					)}
					<div className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight}`}>
						<ResourceToken type="ActionToken" scale={0.9} />
						<span className="gaia-font">
							{availableSpecialActions}/{allSpecialActions}
						</span>
					</div>
					<div className={styles.resourceIndicator}>
						<ResourceToken type="Federations" scale={0.9} />
						<span className="gaia-font">{`${playerState.usableFederations}/${playerState.numFederationTokens}`}</span>
					</div>
					<div className={styles.resourceIndicator + (player.raceId === Race.Ivits ? "" : ` ${styles.invisible}`)}>
						<ResourceToken type="RecordToken" scale={0.7} />
						<span className="gaia-font">{playerState.additionalInfo}</span>
					</div>
				</div>
				<hr />
				{player.raceId !== null && (
					<div className={styles.row}>
						<div
							className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight} ${
								playerState.buildings.mines === MAX_DEPLOYABLE_MINES ? styles.maxedOut : ""
							}`}
						>
							<Building raceId={player.raceId} type={BuildingType.Mine} noAnimation />
							<span className="gaia-font">{playerState.buildings.mines}</span>
							{hasLostPlanet && <span className="gaia-font">+LP</span>}
						</div>
						<div
							className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight} ${
								playerState.buildings.tradingStations === MAX_DEPLOYABLE_TRADING_STATIONS ? styles.maxedOut : ""
							}`}
						>
							<Building raceId={player.raceId} type={BuildingType.TradingStation} noAnimation />
							<span className="gaia-font">{playerState.buildings.tradingStations}</span>
						</div>
						<div
							className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight} ${
								playerState.buildings.researchLabs === MAX_DEPLOYABLE_RESEARCH_LABS ? styles.maxedOut : ""
							}`}
						>
							<Building raceId={player.raceId} type={BuildingType.ResearchLab} noAnimation />
							<span className="gaia-font">{playerState.buildings.researchLabs}</span>
						</div>
						<div className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight} ${playerState.buildings.planetaryInstitute ? styles.maxedOut : ""}`}>
							<Building raceId={player.raceId} type={BuildingType.PlanetaryInstitute} noAnimation />
							<span className="gaia-font">{Number(playerState.buildings.planetaryInstitute)}</span>
						</div>
						<div
							className={`${styles.resourceIndicator} ${styles.resourceIndicatorFixedHeight} ${
								playerState.buildings.academyLeft && playerState.buildings.academyRight ? styles.maxedOut : ""
							}`}
						>
							<Building raceId={player.raceId} type={BuildingType.AcademyLeft} noAnimation />
							<span className="gaia-font">{Number(playerState.buildings.academyLeft) + Number(playerState.buildings.academyRight)}</span>
						</div>
					</div>
				)}
			</div>
		</div>
	);
};

export default observer(PlayerBox);
