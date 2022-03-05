import Button from "@mui/material/Button";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import { Theme } from "@mui/material/styles";
import Typography from "@mui/material/Typography";
import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import _ from "lodash";
import { observer } from "mobx-react";
import { useState } from "react";
import { AvailableActionDto, GameStateDto } from "../../dto/interfaces";
import { centeredFlexRow, fillParent, Nullable } from "../../utils/miscellanea";
import { useGamePageContext } from "../GamePage.context";
import { selectAvailableActions, selectAvailableCommands, selectStatusMessage } from "../store/selectors";
import { useWorkflow } from "../WorkflowContext";
import { Command } from "../workflows/types";
import { fromAction } from "../workflows/utils";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...centeredFlexRow,
			flexDirection: ({ useVerticalLayout }: { useVerticalLayout: boolean }) => (useVerticalLayout ? "column" : "row"),
			...fillParent,
			padding: theme.spacing(1, 2),
			[theme.breakpoints.down("md")]: {
				padding: theme.spacing(0.5, 1),
			},
			backgroundColor: "white",
			boxShadow: "0 3px 3px 3px rgba(0, 0, 0, 0.5)",
		},
		message: {
			flex: "0 1 auto",
			color: "black",
			fontSize: "0.9rem",
			textAlign: "center",
			[theme.breakpoints.down("md")]: {
				fontSize: "0.7rem",
			},
			[theme.breakpoints.down("sm")]: {
				fontSize: "0.6rem",
			},
		},
		actionSelector: {
			flex: "0 0 auto",
			marginLeft: theme.spacing(2),
			[theme.breakpoints.down("md")]: {
				marginLeft: theme.spacing(1),
			},
		},
		actionList: {
			top: "113px !important",
			[theme.breakpoints.down("md")]: {
				top: "100px !important",
			},
			"& .MuiMenuItem-root": {
				fontSize: "0.8rem",
			},
		},
		commands: {
			flex: "0 0 auto",
			marginTop: ({ useVerticalLayout }: { useVerticalLayout: boolean }) => (useVerticalLayout ? theme.spacing(0.25) : 0),
			marginLeft: ({ useVerticalLayout }: { useVerticalLayout: boolean }) => (useVerticalLayout ? 0 : theme.spacing(2)),
			[theme.breakpoints.down("md")]: {
				marginLeft: theme.spacing(1),
			},
			...centeredFlexRow,
		},
		command: {
			fontSize: "0.75rem",
			[theme.breakpoints.down("md")]: {
				padding: theme.spacing(0.5, 1),
				fontSize: "0.6rem",
			},
			"&:not(:last-child)": {
				marginRight: theme.spacing(1),
			},
		},
	})
);

interface StatusBarProps {
	game: GameStateDto;
	isMobile: boolean;
	playerId: Nullable<string>;
	isSpectator: boolean;
}

const StatusBar = ({ game, playerId, isSpectator, isMobile }: StatusBarProps) => {
	const { vm } = useGamePageContext();
	const statusMessage = selectStatusMessage(vm);
	const commands = selectAvailableCommands(vm);
	const availableActions = selectAvailableActions(vm);
	const isExecutingAction = vm.isExecutingAction;
	const isIdle = !isExecutingAction;
	const statusBarMessage = isExecutingAction ? "Executing..." : statusMessage;
	const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
	const { activeWorkflow, startWorkflow } = useWorkflow();
	const useVerticalLayout = isMobile && commands.length > 1;
	const classes = useStyles({ useVerticalLayout });
	const isActivePlayer = game.activePlayer?.id === playerId;
	const showActionSelector = isActivePlayer && !activeWorkflow;

	const handleCommand = (command: Command) => {
		if (!activeWorkflow) {
			throw new Error("How can you issue a command without an active workflow?!");
		}
		const action = activeWorkflow.handleCommand(command);
		if (!action) {
			return;
		}
		vm.executePlayerAction(game.id, action);
	};

	const onActionSelected = (action: AvailableActionDto) => {
		if (isSpectator) {
			return;
		}
		const workflow = fromAction(playerId!, game, action, vm);
		startWorkflow(workflow);
		setMenuAnchor(null);
	};

	return (
		<div className={classes.root}>
			<Typography variant="body1" component="div">
				<div className={classes.message + " gaia-font"}>{statusBarMessage}</div>
			</Typography>
			{!isSpectator && (
				<>
					{isIdle && (
						<div className={classes.commands}>
							{_.map(commands, cmd => (
								<Button
									key={`${cmd.nextState}§${cmd.text}`}
									className={classes.command}
									onClick={() => handleCommand(cmd)}
									variant="contained"
									color={cmd.isPrimary ? "primary" : undefined}
								>
									<span className="gaia-font">{cmd.text}</span>
								</Button>
							))}
						</div>
					)}
					{showActionSelector && (
						<div className={classes.actionSelector}>
							<Button className={classes.command} onClick={evt => setMenuAnchor(evt.currentTarget)} variant="contained" color="primary">
								<span className="gaia-font">Actions</span>
							</Button>
							<Menu
								anchorEl={menuAnchor}
								keepMounted
								open={!!menuAnchor}
								onClose={() => setMenuAnchor(null)}
								PaperProps={{
									className: classes.actionList,
								}}
							>
								{_.map(availableActions, action => (
									<MenuItem key={action.type} onClick={() => onActionSelected(action)}>
										<span className="gaia-font">{action.description}</span>
									</MenuItem>
								))}
							</Menu>
						</div>
					)}
				</>
			)}
		</div>
	);
};

export default observer(StatusBar);
