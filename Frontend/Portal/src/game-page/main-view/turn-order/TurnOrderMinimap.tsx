import { useTheme } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import _ from "lodash";
import { useSelector } from "react-redux";
import { GamePhase, Race } from "../../../dto/enums";
import { GameStateDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { isAuctionOngoing, isLastRound, Nullable } from "../../../utils/miscellanea";
import { getRaceImage } from "../../../utils/race-utils";
import { selectSortedActivePlayers, selectSortedPassedPlayers } from "../../store/active-game.slice";
import { RACE_AVATAR_WIDTH } from "../main-view.styles";
import useStyles from "./turn-order-minimap.styles";

const OngoingAuction = ({ race, username, orderLabel }: { race: Race; username: string; orderLabel: string }) => {
	const theme = useTheme();
	const imgUrl = useAssetUrl(`Races/${getRaceImage(race)}`);
	return (
		<div style={{ display: "flex", alignItems: "center", justifyContent: "flex-start" }}>
			<div style={{ position: "relative" }}>
				<div className="gaia-font" style={{ position: "absolute", bottom: 0, left: 0, color: "white" }}>
					{orderLabel}
				</div>
				<img style={{ width: RACE_AVATAR_WIDTH }} src={imgUrl} alt="" />
			</div>
			<Typography
				variant="caption"
				className="gaia-font"
				style={{ marginLeft: theme.spacing(1), maxWidth: 100, overflowX: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}
			>
				{username.substr(0, 3)}
			</Typography>
		</div>
	);
};

const PlayerAvatar = ({ race, username, orderLabel }: { race: Nullable<Race>; username: string; orderLabel: string }) => {
	const imgUrl = useAssetUrl(`Races/${getRaceImage(race)}`);
	return race ? (
		<div style={{ position: "relative" }}>
			<div className="gaia-font" style={{ position: "absolute", bottom: 0, left: 0, color: "white" }}>
				{orderLabel}
			</div>
			<img style={{ width: RACE_AVATAR_WIDTH }} src={imgUrl} alt="" />
		</div>
	) : (
		<Typography variant="body1" className="gaia-font">
			{username.substr(0, 3)}
		</Typography>
	);
};

interface TurnOrderMinimapProps {
	game: GameStateDto;
	direction: "vertical" | "horizontal";
}

const TurnOrderMinimap = ({ game, direction }: TurnOrderMinimapProps) => {
	const classes = useStyles();
	const isAuctioning = isAuctionOngoing(game);
	const isLastRound_ = isLastRound(game);
	const activePlayers = useSelector(selectSortedActivePlayers);
	const passedPlayers = useSelector(selectSortedPassedPlayers);

	if (isAuctioning) {
		return (
			<div className={classes.root}>
				<div>
					<div className="gaia-font">Auction</div>
					<div className={direction === "vertical" ? classes.playersColumn : classes.playersRow}>
						{_.map(game.auctionState!.auctionedRaces, (auction, index) => (
							<div key={index} className={classes.ongoingAuction}>
								<OngoingAuction race={auction.race} username={auction.playerUsername ?? "No bid"} orderLabel={`${auction.order + 1}°`} />
							</div>
						))}
					</div>
				</div>
			</div>
		);
	}

	return (
		<div className={classes.root}>
			<div>
				<Typography variant="body2" className="gaia-font">
					Round {game.currentRound}
				</Typography>
				<div className={direction === "vertical" ? classes.playersColumn : classes.playersRow}>
					{_.map(activePlayers, p => (
						<div key={p.id} className={`${classes.avatar} ${direction}`}>
							<PlayerAvatar race={p.raceId} username={p.username} orderLabel={`${p.state?.currentRoundTurnOrder ?? 0}°`} />
						</div>
					))}
				</div>
			</div>
			{game.currentPhase !== GamePhase.Setup && !isLastRound_ && _.some(passedPlayers) && (
				<div className={classes.nextRound}>
					<Typography variant="body2" className="gaia-font">
						Round {game.currentRound + 1}
					</Typography>
					<div className={direction === "vertical" ? classes.playersColumn : classes.playersRow}>
						{_.map(passedPlayers, pp => (
							<div key={pp.id} className={`${classes.avatar} ${direction}`}>
								<PlayerAvatar race={pp.raceId} username={pp.username} orderLabel={`${pp.state!.nextRoundTurnOrder}°`} />
							</div>
						))}
					</div>
				</div>
			)}
		</div>
	);
};

export default TurnOrderMinimap;
