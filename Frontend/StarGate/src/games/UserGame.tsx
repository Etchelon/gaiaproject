import { useTheme } from "@material-ui/core";
import Avatar from "@material-ui/core/Avatar";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import { format, parseISO } from "date-fns";
import _ from "lodash";
import { useSelector } from "react-redux";
import { useHistory } from "react-router-dom";
import ActivePlayerImg from "../assets/Resources/PlayerLoader.gif";
import { getRaceColor } from "../utils/race-utils";
import { selectGame } from "./store/games.slice";

const AVATAR_WIDTH = 40;

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		wrapper: {
			padding: theme.spacing(2),
			cursor: "pointer",
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
}

const UserGame = ({ id }: UserGameProps) => {
	const theme = useTheme();
	const classes = useStyles();
	const history = useHistory();
	const game = useSelector(_.partialRight(selectGame, id))!;

	const navigateToGamePage = () => {
		history.push(`/game/${game.id}`);
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
