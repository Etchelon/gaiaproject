import DeleteIcon from "@mui/icons-material/Delete";
import { useTheme } from "@mui/material";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import Grid from "@mui/material/Grid";
import IconButton from "@mui/material/IconButton";
import Paper from "@mui/material/Paper";
import { Theme } from "@mui/material/styles";
import Typography from "@mui/material/Typography";
import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import { format, parseISO } from "date-fns";
import { chain, isNil } from "lodash";
import { observer } from "mobx-react";
import { MouseEvent, MouseEventHandler, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ActivePlayerImg from "../assets/Resources/PlayerLoader.gif";
import { UserInfoDto } from "../dto/interfaces";
import ButtonWithProgress from "../utils/ButtonWithProgress";
import { getRaceColor } from "../utils/race-utils";
import { useGamesContext } from "./UserGames.context";

const AVATAR_WIDTH = 40;

function withStopPropagation(handler: MouseEventHandler) {
	return (evt: MouseEvent) => {
		evt.stopPropagation();
		handler(evt);
	};
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		wrapper: {
			padding: theme.spacing(2),
			cursor: "pointer",
		},
		header: {
			display: "flex",
		},
		info: {
			flexGrow: 1,
			minWidth: 0,
		},
		deleteBtn: {
			marginLeft: theme.spacing(1),
			flexShrink: 0,
		},
		players: {
			padding: theme.spacing(1, 0),
		},
		player: {
			display: "flex",
			alignItems: "center",
			padding: theme.spacing(2),
		},
		playerInfo: {
			width: `calc(100% - ${theme.spacing(1)} - ${AVATAR_WIDTH}px)`,
			marginLeft: theme.spacing(2),
		},
	})
);

interface UserGameProps {
	id: string;
	user: UserInfoDto;
	doDeleteGame(gameId: string): void;
}

const UserGame = ({ id, user, doDeleteGame }: UserGameProps) => {
	const theme = useTheme();
	const classes = useStyles();
	const navigate = useNavigate();
	const { vm } = useGamesContext();
	const [isPromptingForDeletion, setIsPromptingForDeletion] = useState(false);
	const deleteProgress = vm.deleteGameProgress;
	const game = vm.games.find(g => g.id === id)!;
	const isGameCreator = user?.id === game.createdBy.id;
	const canDelete = isGameCreator && !game.ended;

	useEffect(() => {
		if (deleteProgress === "success" || deleteProgress === "failure") {
			setIsPromptingForDeletion(false);
		}
	}, [deleteProgress]);

	const navigateToGamePage = () => {
		navigate(`/game/${game.id}`);
	};
	const creationDate = parseISO(game.created);
	const finishDate = game.ended ? parseISO(game.ended) : null;
	const winnersNames = finishDate
		? chain(game.players)
				.filter(p => p.placement === 1)
				.map(p => p.username)
				.value()
				.join(", ")
		: null;

	return (
		<Paper className={classes.wrapper} onClick={navigateToGamePage}>
			<div className={classes.header}>
				<div className={classes.info}>
					<Typography variant="h5" className="gaia-font">
						{game.name}
					</Typography>
					<Typography variant="subtitle2" className="gaia-font">
						{finishDate ? (
							<span>
								Finished on {format(finishDate, "MMM d, y")}, {winnersNames} won!
							</span>
						) : (
							<span>
								Started on {format(creationDate, "MMM d, y")} by {game.createdBy.username}
							</span>
						)}
					</Typography>
				</div>
				{canDelete && (
					<div className={classes.deleteBtn}>
						<IconButton
							aria-label="Delete this game"
							onClick={withStopPropagation(() => {
								setIsPromptingForDeletion(true);
							})}
							size="large"
						>
							<DeleteIcon />
							<Dialog
								open={isPromptingForDeletion}
								onClose={() => setIsPromptingForDeletion(false)}
								aria-labelledby="alert-dialog-title"
								aria-describedby="alert-dialog-description"
							>
								<DialogTitle id="alert-dialog-title">Delete this game?</DialogTitle>
								<DialogContent>
									<DialogContentText id="alert-dialog-description">You can delete this game. Other players will be notified of this action.</DialogContentText>
								</DialogContent>
								<DialogActions>
									<Button onClick={withStopPropagation(() => setIsPromptingForDeletion(false))} disabled={deleteProgress === "loading"}>
										Cancel
									</Button>
									<ButtonWithProgress
										label={"Yes"}
										loading={deleteProgress === "loading"}
										onClick={withStopPropagation(() => {
											doDeleteGame(game.id);
										})}
										disabled={deleteProgress === "loading"}
										autoFocus
									/>
								</DialogActions>
							</Dialog>
						</IconButton>
					</div>
				)}
			</div>
			<div className={classes.players}>
				<Grid container spacing={2}>
					{chain(game.players)
						.map(p => ({ p, index: !isNil(p.placement) ? 4 - p.placement : p.points }))
						.orderBy(o => o.index, "desc")
						.map(o => o.p)
						.map((player, index) => {
							const playerColor = getRaceColor(player.raceId);
							const ret = (
								<Grid key={player.id} item xs={12} sm={6} md={3}>
									<Paper
										className={classes.player}
										style={{
											backgroundColor: playerColor,
											color: theme.palette.getContrastText(playerColor),
										}}
									>
										<Avatar alt={player.username} src={player.isActive ? ActivePlayerImg : player.avatarUrl} />
										<div className={classes.playerInfo}>
											<Typography variant="body1" className="gaia-font ellipsify">
												{`${finishDate ? `${player.placement ?? index + 1}° - ` : ""}${player.username}`}
											</Typography>
											<Typography variant="caption" className="gaia-font">
												{`${player.raceName} - ${player.points} VP`}
											</Typography>
										</div>
									</Paper>
								</Grid>
							);
							return ret;
						})
						.value()}
				</Grid>
			</div>
		</Paper>
	);
};

export default observer(UserGame);
