import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import { cloneDeep, isEmpty } from "lodash";
import { useReducer, useState } from "react";
import ConversionsPanelImg from "../../../assets/Resources/TerransConversions.png";
import { Conversion } from "../../../dto/enums";
import { PlayerInGameDto, ResourcesDto } from "../../../dto/interfaces";
import styles from "../../game-board/player-box/PlayerBox.module.scss";
import ResourceToken from "../../game-board/ResourceToken";
import { useGamePageContext } from "../../GamePage.context";
import { useWorkflow } from "../../WorkflowContext";
import { TerransDecideIncomeWorkflow } from "../../workflows/rounds-phase/terrans-decide-income.workflow";
import { CommonWorkflowStates } from "../../workflows/types";
import ClickableRectangle from "../ClickableRectangle";
import useStyles from "./terrans-decide-income-dialog.styles";

const sizeAndPosition = (width: number, height: number, top: number, left: number) => ({
	width: `${width}%`,
	height: `${height}%`,
	top: `${top}%`,
	left: `${left}%`,
});

const ores = (resources: ResourcesDto) => resources.ores;
const credits = (resources: ResourcesDto) => resources.credits;
const knowledge = (resources: ResourcesDto) => resources.knowledge;
const qic = (resources: ResourcesDto) => resources.qic;

interface PlayerWithConversions {
	resources: ResourcesDto;
	remainingPower: number;
	conversions: Conversion[];
}

const reducer = (state: PlayerWithConversions, action: { type: Conversion | "reset"; data?: any }): PlayerWithConversions => {
	const newState = cloneDeep(state);
	const resources = newState.resources;

	const spendPower = (amount: number) => {
		newState.remainingPower -= amount;
	};

	switch (action.type) {
		case "reset":
			return { resources: cloneDeep(action.data.resources as ResourcesDto), remainingPower: action.data.remainingPower, conversions: [] };
		case Conversion.PowerToQic:
			resources.qic += 1;
			spendPower(4);
			break;
		case Conversion.PowerToOre:
			resources.ores += 1;
			spendPower(3);
			break;
		case Conversion.PowerToKnowledge:
			resources.knowledge += 1;
			spendPower(4);
			break;
		case Conversion.PowerToCredit:
			spendPower(1);
			resources.credits += 1;
			break;
		default:
			throw new Error(`Action ${action} not implemented.`);
	}
	newState.conversions.push(action.type);
	return newState;
};

interface TerransDecideIncomeProps {
	gameId: string;
	currentPlayer: PlayerInGameDto;
}

const TerransDecideIncome = ({ gameId, currentPlayer }: TerransDecideIncomeProps) => {
	const classes = useStyles();
	const { vm } = useGamePageContext();
	const { activeWorkflow } = useWorkflow();
	const tdiWorkflow = activeWorkflow as TerransDecideIncomeWorkflow;
	const powerToConvert = tdiWorkflow?.powerToConvert ?? 0;
	const [isPerformingConversion, setIsPerformingConversion] = useState(false);

	const [conversionsState, dispatch] = useReducer(reducer, {
		resources: cloneDeep(currentPlayer.state.resources),
		remainingPower: powerToConvert,
		conversions: [],
	});
	const resources = conversionsState.resources;
	const remainingPower = conversionsState.remainingPower;

	const closeDialog = () => {
		activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};

	const convert = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: conversionsState.conversions })!;
		vm.executePlayerAction(gameId, action);
		setIsPerformingConversion(true);
	};

	return (
		<div className={classes.root}>
			<Grid container spacing={2}>
				<Grid item xs={12} md={6}>
					<div className={classes.conversions}>
						<img src={ConversionsPanelImg} alt="" />
						<ClickableRectangle style={{ ...sizeAndPosition(16, 16, 11, 60) }} active={remainingPower >= 4} onClick={() => dispatch({ type: Conversion.PowerToQic })} />
						<ClickableRectangle style={{ ...sizeAndPosition(16, 16, 34, 60) }} active={remainingPower >= 3} onClick={() => dispatch({ type: Conversion.PowerToOre })} />
						<ClickableRectangle
							style={{ ...sizeAndPosition(16, 16, 55, 60) }}
							active={remainingPower >= 4}
							onClick={() => dispatch({ type: Conversion.PowerToKnowledge })}
						/>
						<ClickableRectangle
							style={{ ...sizeAndPosition(16, 16, 76, 60) }}
							active={remainingPower >= 1}
							onClick={() => dispatch({ type: Conversion.PowerToCredit })}
						/>
					</div>
				</Grid>
				<Grid item xs={12} md={6}>
					<div className={classes.status}>
						<div className={`${styles.playerData} ${classes.playerData}`}>
							<Typography variant="h6" className="gaia-font text-center">
								Remaining Power
							</Typography>
							<div className={`${styles.row} ${classes.statusRow}`}>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Power" />
									<span className="gaia-font">{remainingPower}</span>
								</div>
							</div>
							<Typography variant="h6" className="gaia-font text-center">
								Resources
							</Typography>
							<div className={`${styles.row} ${classes.statusRow}`}>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Ores" />
									<span className="gaia-font">{ores(resources)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Credits" />
									<span className="gaia-font">{credits(resources)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Knowledge" />
									<span className="gaia-font">{knowledge(resources)}</span>
								</div>
								<div className={styles.resourceIndicator}>
									<ResourceToken type="Qic" />
									<span className="gaia-font">{qic(resources)}</span>
								</div>
							</div>
						</div>
						<div className={classes.commands}>
							<Button
								variant="contained"
								className="command"
								onClick={() => dispatch({ type: "reset", data: { resources: currentPlayer.state.resources, remainingPower: powerToConvert } })}
							>
								<span className="gaia-font">Reset</span>
							</Button>
							<Button variant="contained" className="command" onClick={closeDialog}>
								<span className="gaia-font">Close</span>
							</Button>
							<Button
								variant="contained"
								color="primary"
								className="command"
								disabled={isPerformingConversion || isEmpty(conversionsState.conversions)}
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

export default TerransDecideIncome;
