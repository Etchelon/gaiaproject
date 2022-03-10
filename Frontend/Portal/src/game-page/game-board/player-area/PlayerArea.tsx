import { isEmpty, isNil } from "lodash";
import { PlayerInGameDto } from "../../../dto/interfaces";
import FederationToken from "../federation-token/FederationToken";
import PlayerBoard from "../player-board/PlayerBoard";
import RoundBooster from "../round-booster/RoundBooster";
import useStyles from "./player-area.styles";
import PlayerTechnologyTile from "./PlayerTechnologyTile";

interface PlayerAreaProps {
	player: PlayerInGameDto;
	framed: boolean;
}

const PlayerArea = ({ player, framed }: PlayerAreaProps) => {
	const classes = useStyles();

	if (isNil(player.state)) {
		return <div></div>;
	}

	return (
		<div className={classes.root + (framed ? " with-frame" : "")}>
			<div className={classes.upper}>
				<div className={classes.board}>
					<PlayerBoard player={player} />
				</div>
				<div className="spacer"></div>
				<div className={classes.boosterAndFederations}>
					{!!player.state.roundBooster && (
						<div className={classes.booster}>
							<RoundBooster booster={player.state.roundBooster} withPlayerInfo={true} />
						</div>
					)}
					<div className={classes.federations}>
						{player.state.federationTokens.map((token, index) => (
							<div key={index} className={classes.federation}>
								<FederationToken type={token.id} playerId={player.id} used={token.usedForTechOrAdvancedTile} />
							</div>
						))}
					</div>
				</div>
			</div>
			{!isEmpty(player.state.technologyTiles) && (
				<div className={classes.techTiles}>
					{player.state.technologyTiles.map(tile => (
						<div key={tile.id} className={classes.techTile}>
							<PlayerTechnologyTile tile={tile} playerId={player.id} />
						</div>
					))}
				</div>
			)}
		</div>
	);
};

export default PlayerArea;
