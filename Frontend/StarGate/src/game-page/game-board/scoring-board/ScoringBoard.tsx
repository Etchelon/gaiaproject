import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import _ from "lodash";
import { FederationTokenStackDto, RoundBoosterTileDto, ScoringTrackDto } from "../../../dto/interfaces";
import FederationTokenStack from "../federation-token/FederationTokenStack";
import RoundBooster from "../round-booster/RoundBooster";
import ScoringTrack from "../scoring-track/ScoringTrack";
import useStyles from "./scoring-board.styles";

interface ScoringBoardProps {
	board: ScoringTrackDto;
	roundBoosters: RoundBoosterTileDto[];
	federationTokens: FederationTokenStackDto[];
	isMobile: boolean;
}

const ScoringBoard = ({ board, roundBoosters, federationTokens, isMobile }: ScoringBoardProps) => {
	const classes = useStyles({ isMobile });

	return (
		<div className={classes.root}>
			<Container maxWidth="lg" disableGutters={isMobile}>
				<Grid container spacing={isMobile ? 0 : 2}>
					<Grid item xs={12} md={6}>
						<ScoringTrack board={board} />
					</Grid>
					<Grid item xs={12} md={6}>
						<div className={classes.roundBoosters}>
							{_.map(roundBoosters, booster => (
								<div key={booster.id} className="booster">
									<RoundBooster booster={booster} withPlayerInfo={false} />
								</div>
							))}
						</div>
					</Grid>
					<Grid item xs={12}>
						<div className={classes.federationTokens}>
							{_.map(federationTokens, stack => (
								<div key={stack.type} className="stack">
									<FederationTokenStack stack={stack} />
								</div>
							))}
						</div>
					</Grid>
				</Grid>
			</Container>
		</div>
	);
};

export default ScoringBoard;
