import { useTheme } from "@material-ui/core";
import Avatar from "@material-ui/core/Avatar";
import Grid from "@material-ui/core/Grid";
import IconButton from "@material-ui/core/IconButton";
import DeleteIcon from "@material-ui/icons/Delete";
import Paper from "@material-ui/core/Paper";
import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import { format, parseISO } from "date-fns";
import _ from "lodash";
import { MouseEvent, MouseEventHandler, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import ActivePlayerImg from "../assets/Resources/PlayerLoader.gif";
import { getRaceColor } from "../utils/race-utils";
import { selectDeleteGameProgress, selectGame } from "./store/games.slice";
import { UserInfoDto } from "../dto/interfaces";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import ButtonWithProgress from "../utils/ButtonWithProgress";

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
			width: `calc(100% - ${theme.spacing(1)}px - ${AVATAR_WIDTH}px)`,
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
	const game = useSelector(_.partialRight(selectGame, id))!;
	const [isPromptingForDeletion, setIsPromptingForDeletion] = useState(false);
	const deleteProgress = useSelector(selectDeleteGameProgress);
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
		? _.chain(game.players)
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
									<Button onClick={withStopPropagation(() => setIsPromptingForDeletion(false))} color="default" disabled={deleteProgress === "loading"}>
										Cancel
									</Button>
									<ButtonWithProgress
										label={"Yes"}
										loading={deleteProgress === "loading"}
										onClick={withStopPropagation(() => {
											doDeleteGame(game.id);
										})}
										color="default"
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
					{_.chain(game.players)
						.map(p => ({ p, index: !_.isNil(p.placement) ? 4 - p.placement : p.points }))
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

export default UserGame;
