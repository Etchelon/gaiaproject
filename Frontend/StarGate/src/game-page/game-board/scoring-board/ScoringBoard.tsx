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
}

const ScoringBoard = (props: ScoringBoardProps) => {
	const classes = useStyles();

	return (
		<div className={classes.root}>
			<Container maxWidth="lg">
				<Grid container spacing={2}>
					<Grid item xs={12} md={6}>
						<ScoringTrack board={props.board} />
					</Grid>
					<Grid item xs={12} md={6}>
						<div className={classes.roundBoosters}>
							{_.map(props.roundBoosters, booster => (
								<div key={booster.id} className="booster">
									<RoundBooster booster={booster} withPlayerInfo={false} />
								</div>
							))}
						</div>
					</Grid>
					<Grid item xs={12}>
						<div className={classes.federationTokens}>
							{_.map(props.federationTokens, stack => (
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
