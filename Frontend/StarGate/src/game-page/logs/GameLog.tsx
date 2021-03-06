import { useTheme } from "@material-ui/core";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import IconButton from "@material-ui/core/IconButton";
import ListItem from "@material-ui/core/ListItem";
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemText from "@material-ui/core/ListItemText";
import Typography from "@material-ui/core/Typography";
import HistoryIcon from "@material-ui/icons/History";
import _ from "lodash";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { GameLogDto, GameSubLogDto } from "../../dto/interfaces";
import ButtonWithProgress from "../../utils/ButtonWithProgress";
import { useAssetUrl } from "../../utils/hooks";
import { getRaceColor, getRaceImage } from "../../utils/race-utils";
import { selectRollbackProgress } from "../store/active-game.slice";
import styles from "./GameLog.module.scss";

interface SystemLogProps {
	message: string;
}

const SystemLog = ({ message }: SystemLogProps) => {
	return (
		<ListItem className={styles.systemLog}>
			<ListItemText primary={message} primaryTypographyProps={{ className: "gaia-font text-center" }} />
		</ListItem>
	);
};

interface PlayerSubLogProps {
	subLog: GameSubLogDto;
	isAnotherPlayer?: boolean;
}

const PlayerSubLog = ({ subLog, isAnotherPlayer }: PlayerSubLogProps) => {
	const theme = useTheme();
	const imgUrl = useAssetUrl(`Races/${getRaceImage(subLog.race)}`);
	const background = getRaceColor(subLog.race);
	const color = theme.palette.getContrastText(background);

	return (
		<div className={`${styles.playerSubLog} ${isAnotherPlayer ? styles.anotherPlayer : ""}`} style={{ backgroundColor: background, color }}>
			<Avatar src={imgUrl} className={styles.avatar} />
			<Typography variant="caption" className={`${styles.mainLog} ${"gaia-font"}`}>
				{subLog.message}
			</Typography>
		</div>
	);
};

interface PlayerLogProps {
	log: GameLogDto;
	canRollback: boolean;
	doRollback(actionId: number): void;
}

const PlayerLog = ({ log, canRollback, doRollback }: PlayerLogProps) => {
	const theme = useTheme();
	const imgUrl = useAssetUrl(`Races/${getRaceImage(log.race)}`);
	const [isPromptingForRollback, setIsPromptingForRollback] = useState(false);
	const rollbackProgress = useSelector(selectRollbackProgress);
	const background = getRaceColor(log.race);
	const color = theme.palette.getContrastText(background);

	useEffect(() => {
		if (isPromptingForRollback && (rollbackProgress === "success" || rollbackProgress === "failure")) {
			setIsPromptingForRollback(false);
		}
	}, [isPromptingForRollback, rollbackProgress]);

	const subLogs = () => (
		<>
			{_.map(log?.subLogs, (sl, index) => (
				<PlayerSubLog key={index} subLog={sl} isAnotherPlayer={sl.player !== log.player}></PlayerSubLog>
			))}
		</>
	);
	return (
		<ListItem className={styles.playerLog} style={{ backgroundColor: background, color }}>
			<ListItemAvatar>
				<Avatar src={imgUrl} />
			</ListItemAvatar>
			<ListItemText
				style={{ margin: theme.spacing(0) }}
				primary={log.message}
				primaryTypographyProps={{ className: `${styles.mainLog} ${"gaia-font"}` }}
				secondary={_.some(log.subLogs) && subLogs()}
				secondaryTypographyProps={{ component: "div" }}
			/>
			{canRollback && (
				<div className={styles.rollbackButton}>
					<IconButton aria-label="Rollback to this action" onClick={() => setIsPromptingForRollback(true)}>
						<HistoryIcon style={{ color }} />
					</IconButton>
					<Dialog
						open={isPromptingForRollback}
						onClose={() => setIsPromptingForRollback(false)}
						aria-labelledby="alert-dialog-title"
						aria-describedby="alert-dialog-description"
					>
						<DialogTitle id="alert-dialog-title">Rollback game state?</DialogTitle>
						<DialogContent>
							<DialogContentText id="alert-dialog-description">The game state will be rolled back to just after the selected action was performed</DialogContentText>
						</DialogContent>
						<DialogActions>
							<Button onClick={() => setIsPromptingForRollback(false)} color="default" disabled={rollbackProgress !== "idle"}>
								Cancel
							</Button>
							<ButtonWithProgress
								label={"Yes"}
								loading={rollbackProgress === "loading"}
								onClick={() => doRollback(log.actionId!)}
								color="default"
								disabled={rollbackProgress !== "idle"}
								autoFocus
							/>
						</DialogActions>
					</Dialog>
				</div>
			)}
		</ListItem>
	);
};

interface GameLogProps {
	log: GameLogDto;
	canRollback: boolean;
	doRollback(actionId: number): void;
}

const GameLog = ({ log, canRollback, doRollback }: GameLogProps) => {
	return log.isSystem ? SystemLog({ message: log.message }) : PlayerLog({ log, canRollback, doRollback });
};

export default GameLog;
