import { CircularProgress } from "@mui/material";
import Fab from "@mui/material/Fab";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import AddIcon from "@mui/icons-material/Add";
import RefreshIcon from "@mui/icons-material/Refresh";
import { differenceInMinutes } from "date-fns";
import _ from "lodash";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { useCurrentUser } from "../utils/hooks";
import { deleteGameAction, fetchGames, GameKind, selectDeleteGameProgress, selectGamesIds, selectGamesStatus, selectLastFetchInfo } from "./store/games.slice";
import useStyles from "./user-games.styles";
import UserGame from "./UserGame";

interface UserGamesProps {
	kind: GameKind;
}

const UserGames = ({ kind }: UserGamesProps) => {
	const user = useCurrentUser()!;
	const gamesIds = useSelector(selectGamesIds);
	const gamesStatus = useSelector(selectGamesStatus);
	const lastFetchInfo = useSelector(selectLastFetchInfo);
	const deleteGameStatus = useSelector(selectDeleteGameProgress);
	const isLoading = gamesStatus === "loading";
	const dispatch = useDispatch();
	const classes = useStyles();

	useEffect(() => {
		const lastFetched = lastFetchInfo?.when;
		const isOutdated = !lastFetched || differenceInMinutes(Date.parse(lastFetched), new Date()) > 5;
		const fetchParams = { kind };
		const shouldFetch = isOutdated || lastFetchInfo.params !== JSON.stringify(fetchParams);
		if (gamesStatus !== "loading" && shouldFetch) {
			dispatch(fetchGames(kind));
		}
	}, [kind, gamesStatus, lastFetchInfo]);

	useEffect(() => {
		if (deleteGameStatus !== "success") {
			return;
		}
		dispatch(fetchGames(kind));
	}, [kind, deleteGameStatus]);

	if (isLoading) {
		return (
			<div className={classes.loader}>
				<CircularProgress />
			</div>
		);
	}

	return (
        <div className={classes.wrapper}>
			<div className={classes.header}>
				<Typography variant="h5" className="gaia-font">
					{kind === "waiting" ? "Games waiting for you" : `Your ${kind} games`}
				</Typography>
				<div className={classes.spacer}></div>
				<div className={classes.actions}>
					<IconButton
                        aria-label="refresh"
                        color="default"
                        onClick={() => dispatch(fetchGames(kind))}
                        size="large">
						<RefreshIcon />
					</IconButton>
				</div>
			</div>
			<div className={classes.games}>
				{_.map(gamesIds, id => (
					<div className="game" key={id}>
						<UserGame id={String(id)} user={user} doDeleteGame={gameId => dispatch(deleteGameAction({ gameId }))} />
					</div>
				))}
			</div>
			<div className={classes.newGame}>
				<Fab size="medium" color="primary" aria-label="new" component={Link} to="/new-game">
					<AddIcon />
				</Fab>
			</div>
		</div>
    );
};

export default UserGames;
