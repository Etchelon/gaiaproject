import { observer } from "mobx-react";
import { GameStateDto } from "../../dto/interfaces";
import FederationTokenStack from "../game-board/federation-token/FederationTokenStack";
import Map from "../game-board/map/Map";
import ResearchBoard from "../game-board/research-board/ResearchBoard";
import RoundBooster from "../game-board/round-booster/RoundBooster";
import ScoringTrack from "../game-board/scoring-track/ScoringTrack";
import { useGamePageContext } from "../GamePage.context";
import { selectAllRoundBoosters } from "../store/selectors";
import { ActiveView } from "../workflows/types";
import useStyles, { BOOSTER_AND_FEDERATION_WIDTH, BOOSTER_SPACING, FEDERATION_SPACING, FEDERATION_WIDTH } from "./main-view.styles";
import TurnOrderMinimap from "./turn-order/TurnOrderMinimap";

interface MainViewProps {
	game: GameStateDto;
	width: number;
	height: number;
	showMinimaps: boolean;
	minimapClicked(view: ActiveView): void;
}

const MainView = ({ game, width, height, showMinimaps, minimapClicked }: MainViewProps) => {
	const { vm } = useGamePageContext();
	const map = game.boardState.map;
	const boosters = selectAllRoundBoosters(vm);
	const federations = game.boardState.availableFederations.filter(stack => stack.remaining > 0);
	const classes = useStyles({ nBoosters: boosters.length, nFederations: federations.length });

	return (
		<div className={classes.root}>
			{showMinimaps && (
				<>
					<div className={`${classes.miniMap} ${classes.scoringTrack}`}>
						<ScoringTrack board={game.boardState.scoringBoard} />
						<div className="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)}></div>
					</div>
					<div className={`${classes.miniMap} zoomable ${classes.researchBoard}`}>
						<ResearchBoard board={game.boardState.researchBoard} width={width * 0.3} height={height * 0.3} />
						<div className="click-trap" onClick={() => minimapClicked(ActiveView.ResearchBoard)}></div>
					</div>
					<div className={`${classes.miniMap} ${classes.boostersAndFederations}`}>
						<div className={classes.roundBoosters}>
							{boosters.map((booster, index) => (
								<div key={booster.id} className={classes.roundBooster} style={{ right: (BOOSTER_AND_FEDERATION_WIDTH + BOOSTER_SPACING) * index }}>
									<RoundBooster booster={booster} withPlayerInfo={true} nonInteractive={true} />
								</div>
							))}
							<div className="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)}></div>
						</div>
						<div className={classes.federations}>
							{federations.map((stack, index) => (
								<div key={stack.type} className={classes.federation} style={{ right: (FEDERATION_WIDTH + FEDERATION_SPACING) * index }}>
									<FederationTokenStack stack={stack} />
								</div>
							))}
							<div className="click-trap" onClick={() => minimapClicked(ActiveView.ScoringBoard)}></div>
						</div>
					</div>
					<div className={`${classes.miniMap} ${classes.turnOrder}`}>
						<TurnOrderMinimap game={game} direction="vertical" />
					</div>
				</>
			)}
			<Map map={map} width={width} height={height} />
		</div>
	);
};

export default observer(MainView);
