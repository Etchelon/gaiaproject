import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import { cloneDeep, isUndefined, size } from "lodash";
import { useReducer, useState } from "react";
import { BrainstoneLocation, Race, SortableIncomeType } from "../../../dto/enums";
import { PlayerInGameDto, PlayerStateDto, SortableIncomeDto } from "../../../dto/interfaces";
import { Nullable } from "../../../utils/miscellanea";
import styles from "../../game-board/player-box/PlayerBox.module.scss";
import ResourceToken from "../../game-board/ResourceToken";
import { useGamePageContext } from "../../GamePage.context";
import { useWorkflow } from "../../WorkflowContext";
import { SortIncomesWorkflow } from "../../workflows/rounds-phase/sort-incomes.workflow";
import { CommonWorkflowStates } from "../../workflows/types";
import useStyles from "./sort-incomes-dialog.styles";

const brainstone = (playerState: PlayerStateDto) => playerState.resources.power.brainstone;
const power1 = (playerState: PlayerStateDto) => playerState.resources.power.bowl1;
const power2 = (playerState: PlayerStateDto) => playerState.resources.power.bowl2;
const power3 = (playerState: PlayerStateDto) => playerState.resources.power.bowl3;
const powerGaia = (playerState: PlayerStateDto) => playerState.resources.power.gaiaArea;

const gainPower = (player: PlayerInGameDto, amount: number) => {
	const playerState = player.state;
	let brainstone_ = brainstone(playerState);
	const involvesBrainstone = player.raceId === Race.Taklons && (brainstone_ === BrainstoneLocation.Bowl1 || brainstone_ === BrainstoneLocation.Bowl2);

	if (involvesBrainstone) {
		if (brainstone_ === BrainstoneLocation.Bowl1) {
			amount -= 1;
			brainstone_ = BrainstoneLocation.Bowl2;
		}
	}
	if (amount === 0) {
		return;
	}

	let power1_ = power1(playerState);
	let power2_ = power2(playerState);
	let power3_ = power3(playerState);

	const setPowerValues = (p1: number, p2: number, p3: number, b: Nullable<BrainstoneLocation>) => {
		playerState.resources.power.bowl1 = p1;
		playerState.resources.power.bowl2 = p2;
		playerState.resources.power.bowl3 = p3;
		playerState.resources.power.brainstone = b;
	};

	if (power1_ > 0) {
		if (amount <= power1_) {
			power1_ -= amount;
			power2_ += amount;
			setPowerValues(power1_, power2_, power3_, brainstone_);
			return;
		} else {
			power2_ += power1_;
			amount -= power1_;
			power1_ = 0;
		}
	}
	if (amount === 0) {
		setPowerValues(power1_, power2_, power3_, brainstone_);
		return;
	}

	if (involvesBrainstone) {
		amount -= 1;
		brainstone_ = BrainstoneLocation.Bowl3;
	}
	if (amount === 0) {
		setPowerValues(power1_, power2_, power3_, brainstone_);
		return;
	}

	if (power2_ > 0) {
		if (amount <= power2_) {
			power2_ -= amount;
			power3_ += amount;
			setPowerValues(power1_, power2_, power3_, brainstone_);
			return;
		} else {
			power3_ += power2_;
			power2_ = 0;
		}
	}

	setPowerValues(power1_, power2_, power3_, brainstone_);
};

interface SortingState {
	player: PlayerInGameDto;
	allIncomes: SortableIncomeDto[];
	sortedIncomes: SortableIncomeDto[];
}

const reducer = (state: SortingState, action: { type: "push" | "reset"; data?: any }): SortingState => {
	const newState = cloneDeep(state);
	const player = newState.player;

	switch (action.type) {
		case "reset":
			return { player: cloneDeep(action.data.player), allIncomes: cloneDeep(action.data.allIncomes), sortedIncomes: [] };
		case "push":
			const income = action.data as SortableIncomeDto;
			newState.sortedIncomes.push(income);
			switch (income.type) {
				case SortableIncomeType.Power:
					gainPower(player, income.amount);
					break;
				case SortableIncomeType.PowerToken:
					let amountToGain = income.amount;
					const involvesBrainstone = player.raceId === Race.Taklons && brainstone(player.state) === BrainstoneLocation.Removed;
					if (involvesBrainstone) {
						amountToGain -= 1;
						player.state.resources.power.brainstone = BrainstoneLocation.Bowl1;
					}
					player.state.resources.power.bowl1 += amountToGain;
					break;
				default:
					throw new Error("Impossible.");
			}
			return newState;
		default:
			throw new Error(`Action ${action.type} not implemented.`);
	}
};

interface SortIncomesDialogProps {
	gameId: string;
	currentPlayer: PlayerInGameDto;
}

const SortIncomesDialog = ({ gameId, currentPlayer }: SortIncomesDialogProps) => {
	const classes = useStyles();
	const { activeWorkflow } = useWorkflow();
	const { vm } = useGamePageContext();

	// TODO: temporarily nullable, until I figure out how to destroy the dialog before closing the workflow
	const sortIncomesWorkflow = activeWorkflow as Nullable<SortIncomesWorkflow>;
	const unsortedIncomes = sortIncomesWorkflow?.unsortedIncomes ?? [];
	const [sortingState, dispatch] = useReducer(reducer, { player: cloneDeep(currentPlayer), allIncomes: cloneDeep(unsortedIncomes), sortedIncomes: [] });
	const playerState = sortingState.player.state;
	const incomes = sortingState.allIncomes;
	const sortedIncomes = sortingState.sortedIncomes;
	const [isExecuting, setIsExecuting] = useState(false);

	const sort = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: sortingState.sortedIncomes.map(si => si.id) })!;
		vm.executePlayerAction(gameId, action);
		setIsExecuting(true);
	};

	return (
		<div className={classes.root}>
			<Grid container spacing={2}>
				<Grid item xs={12} md={4}>
					<div className={classes.incomes}>
						<Typography variant="body1" className="gaia-font text-center">
							Incomes to sort
						</Typography>
						{incomes.map((income, index) => (
							<Button
								key={index}
								variant="contained"
								className={classes.income}
								disabled={!isUndefined(sortedIncomes.find(si => si.id === income.id))}
								onClick={() => dispatch({ type: "push", data: income })}
							>
								<span className="gaia-font">{income.description}</span>
							</Button>
						))}
					</div>
				</Grid>
				<Grid item xs={12} md={8}>
					<div className={classes.status}>
						<div className={`${styles.playerData} ${classes.playerData}`}>
							<Typography variant="h6" className="gaia-font text-center">
								Power Bowls
							</Typography>
							<div className={classes.statusRow}>
								<div className={classes.resource}>
									<Typography variant="caption" className="gaia-font">
										Bowl 1
									</Typography>
									<div className={classes.indicator}>
										<ResourceToken type="Power" />
										<span className="gaia-font">{`${power1(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl1 ? " (B)" : ""}`}</span>
									</div>
								</div>
								<div className={classes.resource}>
									<Typography variant="caption" className="gaia-font">
										Bowl 2
									</Typography>
									<div className={classes.indicator}>
										<ResourceToken type="Power" />
										<span className="gaia-font">{`${power2(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl2 ? " (B)" : ""}`}</span>
									</div>
								</div>
								<div className={classes.resource}>
									<Typography variant="caption" className="gaia-font">
										Bowl 3
									</Typography>
									<div className={classes.indicator}>
										<ResourceToken type="Power" />
										<span className="gaia-font">{`${power3(playerState)}${brainstone(playerState) === BrainstoneLocation.Bowl3 ? " (B)" : ""}`}</span>
									</div>
								</div>
								<div className={classes.resource}>
									<Typography variant="caption" className="gaia-font">
										Gaia
									</Typography>
									<div className={classes.indicator}>
										<ResourceToken type="Power" />
										<span className="gaia-font">{`${powerGaia(playerState)}${brainstone(playerState) === BrainstoneLocation.GaiaArea ? " (B)" : ""}`}</span>
									</div>
								</div>
							</div>
						</div>
						<div className={classes.commands}>
							<Button
								variant="contained"
								className="command"
								onClick={() => dispatch({ type: "reset", data: { player: currentPlayer, allIncomes: unsortedIncomes } })}
							>
								<span className="gaia-font">Reset</span>
							</Button>
							<Button variant="contained" color="primary" className="command" disabled={isExecuting || size(incomes) !== size(sortedIncomes)} onClick={sort}>
								<span className="gaia-font">Confirm</span>
							</Button>
						</div>
					</div>
				</Grid>
			</Grid>
		</div>
	);
};

export default SortIncomesDialog;
