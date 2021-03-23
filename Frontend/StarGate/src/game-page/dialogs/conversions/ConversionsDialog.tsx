import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import _ from "lodash";
import { useReducer, useState } from "react";
import { useDispatch } from "react-redux";
import ConversionsPanelImg from "../../../assets/Resources/Conversions.png";
import { BrainstoneLocation, Conversion, Race } from "../../../dto/enums";
import { PlayerInGameDto, PlayerStateDto } from "../../../dto/interfaces";
import styles from "../../game-board/player-box/PlayerBox.module.scss";
import ResourceToken from "../../game-board/ResourceToken";
import { executePlayerAction } from "../../store/actions-thunks";
import { useWorkflow } from "../../WorkflowContext";
import { CommonWorkflowStates } from "../../workflows/types";
import ClickableRectangle from "../ClickableRectangle";
import useStyles from "./conversions-dialog.styles";

const BRAINSTONE_POWER_VALUE = 3;

const sizeAndPosition = (width: number, height: number, top: number, left: number) => ({
	width: `${width}%`,
	height: `${height}%`,
	top: `${top}%`,
	left: `${left}%`,
});

const ores = (playerState: PlayerStateDto) => playerState.resources.ores;
const credits = (playerState: PlayerStateDto) => playerState.resources.credits;
const knowledge = (playerState: PlayerStateDto) => playerState.resources.knowledge;
const qic = (playerState: PlayerStateDto) => playerState.resources.qic;
const brainstone = (playerState: PlayerStateDto) => playerState.resources.power.brainstone;
const power1 = (playerState: PlayerStateDto) => playerState.resources.power.bowl1;
const power2 = (playerState: PlayerStateDto) => playerState.resources.power.bowl2;
const actualPower2 = (playerState: PlayerStateDto) => power2(playerState) + Number(brainstone(playerState) === BrainstoneLocation.Bowl2);
const power3 = (playerState: PlayerStateDto) => playerState.resources.power.bowl3;
const actualPower3 = (playerState: PlayerStateDto) => power3(playerState) + BRAINSTONE_POWER_VALUE * Number(brainstone(playerState) === BrainstoneLocation.Bowl3);
const powerGaia = (playerState: PlayerStateDto) => playerState.resources.power.gaiaArea;
const availableGaiaformersCount = (playerState: PlayerStateDto) =>
	_.chain(playerState.availableGaiaformers)
		.filter(gf => gf.available)
		.size()
		.value();
const range = (playerState: PlayerStateDto) => playerState.navigationRange;
const rangeBoost = (playerState: PlayerStateDto) => playerState.rangeBoost;

const canBurn = (playerState: PlayerStateDto) => actualPower2(playerState) >= 2;
const hasPlanetaryInstitute = (playerState: PlayerStateDto) => playerState.buildings.planetaryInstitute;
const hasQicAcademy = (playerState: PlayerStateDto) => playerState.buildings.academyRight;
const playerIs = (p: PlayerInGameDto, race: Race) => p.raceId === race;
const isHadschHallasWithPlanetaryInstitute = (player: PlayerInGameDto) => playerIs(player, Race.HadschHallas) && hasPlanetaryInstitute(player.state);
const isNevlasWithPlanetaryInstitute = (player: PlayerInGameDto) => playerIs(player, Race.Nevlas) && hasPlanetaryInstitute(player.state);
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

const reducer = (state: PlayerWithConversions, action: { type: Conversion | "reset"; data?: any }): PlayerWithConversions => {
	const newState = _.cloneDeep(state);
	const player = newState.player;
	const playerState = newState.player.state;
	const resources = newState.player.state.resources;
	const power = newState.player.state.resources.power;

	const spendPower = (amount: number) => {
		if (isTaklonsWithBrainstone(player) && brainstone(playerState) === BrainstoneLocation.Bowl3 && amount >= BRAINSTONE_POWER_VALUE) {
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
			return { player: _.cloneDeep(action.data! as PlayerInGameDto), conversions: [] };
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
			const gfToSpend = _.chain(playerState.availableGaiaformers)
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

interface ConversionsDialogProps {
	gameId: string;
	currentPlayer: PlayerInGameDto;
}

const ConversionsDialog = ({ gameId, currentPlayer }: ConversionsDialogProps) => {
	const classes = useStyles();
	const { activeWorkflow } = useWorkflow();
	const storeDispatch = useDispatch();

	const [isPerformingConversion, setIsPerformingConversion] = useState(false);
	const [conversionsState, dispatch] = useReducer(reducer, { player: _.cloneDeep(currentPlayer), conversions: [] });
	const player = conversionsState.player;
	const playerState = player.state;

	const cancel = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
		action && storeDispatch(executePlayerAction({ gameId, action }));
	};
	const convert = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.PERFORM_CONVERSION, data: conversionsState.conversions })!;
		storeDispatch(executePlayerAction({ gameId, action }));
		setIsPerformingConversion(true);
	};

	return (
		<div className={classes.root}>
			<Grid container spacing={2}>
				<Grid item xs={12} md={6}>
					<div className={classes.conversions}>
						<div className={classes.normalConversions}>
							<img src={ConversionsPanelImg} alt="" />
							<ClickableRectangle
								style={{ ...sizeAndPosition(14, 6.35, 15, 53) }}
								active={qic(playerState) > 0}
								onClick={() => dispatch({ type: Conversion.BoostRange })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(14, 6.35, 30, 53) }}
								active={qic(playerState) > 0}
								onClick={() => dispatch({ type: Conversion.QicToOre })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(14, 6.35, 55, 53) }}
								active={knowledge(playerState) > 0}
								onClick={() => dispatch({ type: Conversion.KnowledgeToCredit })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 23, 34) }}
								active={equivalentPower3(player) >= 4 && !isGleensWithoutQicAcademy(player)}
								onClick={() => dispatch({ type: Conversion.PowerToQic })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 37.5, 34) }}
								active={equivalentPower3(player) >= 3}
								onClick={() => dispatch({ type: Conversion.PowerToOre })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 48, 34) }}
								active={equivalentPower3(player) >= 4}
								onClick={() => dispatch({ type: Conversion.PowerToKnowledge })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 62, 34) }}
								active={equivalentPower3(player) > 0 && !(equivalentPower3(player) === 3 && brainstone(playerState) === BrainstoneLocation.Bowl3)}
								onClick={() => dispatch({ type: Conversion.PowerToCredit })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 70.5, 34) }}
								active={ores(playerState) > 0}
								onClick={() => dispatch({ type: Conversion.OreToCredit })}
							/>
							<ClickableRectangle
								style={{ ...sizeAndPosition(16, 5.9, 84.5, 34) }}
								active={ores(playerState) > 0}
								onClick={() => dispatch({ type: Conversion.OreToPowerToken })}
							/>
						</div>
						<div className={classes.spacer}></div>
						<div className={classes.additionalConversions}>
							<Typography variant="body1" className="gaia-font text-center">
								Other
							</Typography>
							<Button variant="contained" color="default" className={classes.conversion} disabled={!canBurn} onClick={() => dispatch({ type: Conversion.BurnPower })}>
								<span className="gaia-font">Burn 1 Power</span>
							</Button>
							{isNevlasWithPlanetaryInstitute(player) && (
								<>
									<Button
										variant="contained"
										color="default"
										className={classes.conversion}
										disabled={power3(playerState) <= 0}
										onClick={() => dispatch({ type: Conversion.NevlasPower3ToKnowledge })}
									>
										<span className="gaia-font">{"Bowl 3 -> Knowledge"}</span>
									</Button>
									<Button
										variant="contained"
										color="default"
										className={classes.conversion}
										disabled={equivalentPower3(player) < 6}
										onClick={() => dispatch({ type: Conversion.Nevlas3PowerTo2Ores })}
									>
										<span className="gaia-font">{"3 Power -> 2 Ores"}</span>
									</Button>
								</>
							)}
							{isHadschHallasWithPlanetaryInstitute(player) && (
								<>
									<Button
										variant="contained"
										color="default"
										className={classes.conversion}
										disabled={credits(playerState) < 4}
										onClick={() => dispatch({ type: Conversion.HadschHallas4CreditsToQic })}
									>
										<span className="gaia-font">{"4 Credits -> Qic"}</span>
									</Button>
									<Button
										variant="contained"
										color="default"
										className={classes.conversion}
										disabled={credits(playerState) < 4}
										onClick={() => dispatch({ type: Conversion.HadschHallas4CreditsToKnowledge })}
									>
										<span className="gaia-font">{"4 Credits -> Knowledge"}</span>
									</Button>
									<Button
										variant="contained"
										color="default"
										className={classes.conversion}
										disabled={credits(playerState) < 3}
										onClick={() => dispatch({ type: Conversion.HadschHallas3CreditsToOre })}
									>
										<span className="gaia-font">{"3 Credits -> Ore"}</span>
									</Button>
								</>
							)}
							{playerIs(player, Race.BalTaks) && (
								<Button
									variant="contained"
									color="default"
									className={classes.conversion}
									disabled={availableGaiaformersCount(playerState) === 0}
									onClick={() => dispatch({ type: Conversion.BalTaksGaiaformerToQic })}
								>
									<span className="gaia-font">{"Gaiaformer -> Qic"}</span>
								</Button>
							)}
							{playerIs(player, Race.Taklons) && (
								<Button
									variant="contained"
									color="default"
									className={classes.conversion}
									disabled={brainstone(playerState) !== BrainstoneLocation.Bowl3}
									onClick={() => dispatch({ type: Conversion.TaklonsBrainstoneToCredits })}
								>
									<span className="gaia-font">{"Brainstone -> 3 Credits"}</span>
								</Button>
							)}
						</div>
					</div>
				</Grid>
				<Grid item xs={12} md={6}>
					<div className={classes.status}>
						<div className={`${styles.playerData} ${classes.playerData}`}>
							<Typography variant="h6" className="gaia-font text-center">
								Resources
							</Typography>
							<div className={`${styles.row} ${classes.statusRow}`}>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Ores" />
									<span className="gaia-font">{ores(playerState)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Credits" />
									<span className="gaia-font">{credits(playerState)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Knowledge" />
									<span className="gaia-font">{knowledge(playerState)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Qic" />
									<span className="gaia-font">{qic(playerState)}</span>
								</div>
							</div>
							<Typography variant="h6" className="gaia-font text-center">
								Power Bowls
							</Typography>
							<div className={`${styles.row} ${classes.statusRow}`}>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Power" />
									<span className="gaia-font">{`${power1(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Power" />
									<span className="gaia-font">{`${power2(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl2 ? " (B)" : ""}`}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Power" />
									<span className="gaia-font">{`${power3(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl3 ? " (B)" : ""}`}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Power" />
									<span className="gaia-font">{`${powerGaia(playerState)}${brainstone(playerState) === BrainstoneLocation.GaiaArea ? " (B)" : ""}`}</span>
								</div>
							</div>
							<Typography variant="h6" className="gaia-font text-center">
								Skills
							</Typography>
							<div className={`${styles.row} ${classes.statusRow}`} style={{ justifyContent: "space-around" }}>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Navigation" />
									<span className="gaia-font">{`${range(playerState)}${rangeBoost(playerState) ? ` +(${rangeBoost(playerState)})` : ""}`}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Gaiaformation" scale={2} />
									<span className="gaia-font">{availableGaiaformersCount(playerState)}</span>
								</div>
							</div>
						</div>
						<div className={classes.commands}>
							<Button variant="contained" color="default" className="command" onClick={cancel}>
								<span className="gaia-font">Cancel</span>
							</Button>
							<Button variant="contained" color="default" className="command" onClick={() => dispatch({ type: "reset", data: currentPlayer })}>
								<span className="gaia-font">Reset</span>
							</Button>
							<Button
								variant="contained"
								color="primary"
								className="command"
								disabled={isPerformingConversion || _.isEmpty(conversionsState.conversions)}
								onClick={convert}
							>
								<span className="gaia-font">Confirm</span>
							</Button>
						</div>
					</div>
				</Grid>
			</Grid>
		</div>
	);
};

export default ConversionsDialog;
