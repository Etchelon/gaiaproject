import Button from "@material-ui/core/Button";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import _ from "lodash";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { AvailableActionDto, GameStateDto } from "../../dto/interfaces";
import { centeredFlexRow, fillParent } from "../../utils/miscellanea";
import { selectAvailableActions, selectAvailableCommands, selectStatusMessage } from "../store/active-game.slice";
import { executePlayerAction, selectIsExecutingAction } from "../store/actions-thunks";
import { useWorkflow } from "../WorkflowContext";
import { Command } from "../workflows/types";
import { fromAction } from "../workflows/utils";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...centeredFlexRow,
			...fillParent,
			padding: theme.spacing(1, 2),
			[theme.breakpoints.down("sm")]: {
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
			[theme.breakpoints.down("sm")]: {
				fontSize: "0.7rem",
			},
			[theme.breakpoints.down("xs")]: {
				fontSize: "0.6rem",
			},
		},
		actionSelector: {
			flex: "0 0 auto",
			marginLeft: theme.spacing(2),
			[theme.breakpoints.down("sm")]: {
				marginLeft: theme.spacing(1),
			},
		},
		actionList: {
			top: "113px !important",
			[theme.breakpoints.down("sm")]: {
				top: "100px !important",
			},
			"& .MuiMenuItem-root": {
				fontSize: "0.8rem",
			},
		},
		commands: {
			flex: "0 0 auto",
			marginLeft: theme.spacing(2),
			[theme.breakpoints.down("sm")]: {
				marginLeft: theme.spacing(1),
			},
			...centeredFlexRow,
		},
		command: {
			fontSize: "0.75rem",
			[theme.breakpoints.down("sm")]: {
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
	playerId: string;
}

const StatusBar = ({ game, playerId }: StatusBarProps) => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const statusMessage = useSelector(selectStatusMessage);
	const commands = useSelector(selectAvailableCommands);
	const availableActions = useSelector(selectAvailableActions);
	const isExecutingAction = useSelector(selectIsExecutingAction);
	const isIdle = !isExecutingAction;
	const statusBarMessage = isExecutingAction ? "Executing..." : statusMessage;
	const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
	const { activeWorkflow, startWorkflow, closeWorkflow } = useWorkflow();
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
		dispatch(executePlayerAction({ gameId: game.id, action }));
	};

	const onActionSelected = (action: AvailableActionDto) => {
		const workflow = fromAction(playerId, game, action, dispatch);
		startWorkflow(workflow);
		setMenuAnchor(null);
	};

	return (
		<div className={classes.root}>
			<Typography variant="body1" component="div">
				<div className={classes.message + " gaia-font"}>{statusBarMessage}</div>
			</Typography>
			{isIdle && (
				<div className={classes.commands}>
					{_.map(commands, cmd => (
						<Button
							key={`${cmd.nextState}§${cmd.text}`}
							className={classes.command}
							onClick={() => handleCommand(cmd)}
							variant="contained"
							color={cmd.isPrimary ? "primary" : "default"}
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
		</div>
	);
};

export default StatusBar;
