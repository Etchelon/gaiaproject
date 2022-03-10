import AddIcon from "@mui/icons-material/Add";
import RefreshIcon from "@mui/icons-material/Refresh";
import { CircularProgress } from "@mui/material";
import Fab from "@mui/material/Fab";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import { differenceInMinutes } from "date-fns";
import { isEqual } from "lodash";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useCurrentUser } from "../utils/hooks";
import { GameKind } from "./store/types";
import useStyles from "./user-games.styles";
import UserGame from "./UserGame";
import { useGamesContext } from "./UserGames.context";

interface UserGamesProps {
	kind: GameKind;
}

const UserGames = ({ kind }: UserGamesProps) => {
	const user = useCurrentUser()!;
	const { vm } = useGamesContext();
	const gamesIds = vm.gamesIds;
	const gamesStatus = vm.status;
	const lastFetchInfo = { when: vm.lastFetched, params: vm.lastFetchParams };
	const deleteGameStatus = vm.deleteGameProgress;
	const isLoading = vm.isLoading;
	const classes = useStyles();

	useEffect(() => {
		const lastFetched = lastFetchInfo?.when;
		const isOutdated = !lastFetched || differenceInMinutes(Date.parse(lastFetched), new Date()) > 5;
		const fetchParams = { kind };
		const shouldFetch = isOutdated || !isEqual(lastFetchInfo.params, fetchParams);
		if (gamesStatus !== "loading" && shouldFetch) {
			vm.fetchGames(kind);
		}
	}, [kind, gamesStatus, lastFetchInfo]);

	useEffect(() => {
		if (deleteGameStatus !== "success") {
			return;
		}
		vm.fetchGames(kind);
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
					<IconButton aria-label="refresh" color="default" onClick={() => vm.fetchGames(kind)} size="large">
						<RefreshIcon />
					</IconButton>
				</div>
			</div>
			<div className={classes.games}>
				{gamesIds.map(id => (
					<div className="game" key={id}>
						<UserGame id={String(id)} user={user} doDeleteGame={gameId => vm.deleteGame(gameId)} />
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

export default observer(UserGames);
