import { CircularProgress } from "@material-ui/core";
import Fab from "@material-ui/core/Fab";
import IconButton from "@material-ui/core/IconButton";
import Typography from "@material-ui/core/Typography";
import AddIcon from "@material-ui/icons/Add";
import RefreshIcon from "@material-ui/icons/Refresh";
import { differenceInMinutes } from "date-fns";
import _ from "lodash";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { fetchGames, GameKind, selectGamesIds, selectGamesStatus, selectLastFetchInfo } from "./store/games.slice";
import useStyles from "./user-games.styles";
import UserGame from "./UserGame";

interface UserGamesProps {
	kind: GameKind;
}

const UserGames = ({ kind }: UserGamesProps) => {
	const gamesIds = useSelector(selectGamesIds);
	const gamesStatus = useSelector(selectGamesStatus);
	const lastFetchInfo = useSelector(selectLastFetchInfo);
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
					<IconButton aria-label="refresh" color="default" onClick={() => dispatch(fetchGames(kind))}>
						<RefreshIcon />
					</IconButton>
				</div>
			</div>
			<div className={classes.games}>
				{_.map(gamesIds, id => (
					<div className="game" key={id}>
						<UserGame id={String(id)} />
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
