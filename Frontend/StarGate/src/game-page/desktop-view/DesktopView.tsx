import { Grid, Tab, Tabs } from "@material-ui/core";
import _ from "lodash";
import { useLayoutEffect, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { GamePhase } from "../../dto/enums";
import { ElementSize } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import PlayerConfig from "../config/PlayerConfig";
import PlayerAreas from "../game-board/player-area/PlayerAreas";
import PlayerBox from "../game-board/player-box/PlayerBox";
import ResearchBoard from "../game-board/research-board/ResearchBoard";
import ScoringBoard from "../game-board/scoring-board/ScoringBoard";
import { GameViewProps } from "../GamePage";
import GameLog from "../logs/GameLog";
import MainView from "../main-view/MainView";
import { setActiveView } from "../store/active-game.slice";
import { rollbackGameAtAction } from "../store/actions-thunks";
import { ActiveView } from "../workflows/types";
import useStyles from "./desktop-view.styles";

const DesktopView = ({ game, currentPlayerId, players, activeView }: GameViewProps) => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const activeViewContainerRef = useRef<HTMLDivElement>(null);
	const [activeViewDimensions, setActiveViewDimensions] = useState<Nullable<ElementSize>>(null);
	const isGameCreator = game.createdBy.id === currentPlayerId;
	const canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;

	useLayoutEffect(() => {
		if (_.isNil(activeViewContainerRef.current)) {
			return;
		}

		setActiveViewDimensions({
			width: activeViewContainerRef.current.offsetWidth,
			height: activeViewContainerRef.current.offsetHeight,
		});
	}, [activeViewContainerRef, game]);

	const PlayerBoxesAndLogs = (
		<div className={classes.playerBoxesAndLogs}>
			{_.map(players, (p, index) => (
				<div key={p.id} className={classes.playerBox}>
					<PlayerBox player={p} index={index + 1} />
				</div>
			))}
			{_.map([...game.gameLogs].reverse(), (log, index) => (
				<div key={index} className={classes.gameLog}>
					<GameLog log={log} canRollback={canRollback} doRollback={actionId => dispatch(rollbackGameAtAction({ gameId: game.id, actionId }))} />
				</div>
			))}
		</div>
	);

	return (
		<Grid container className={classes.root}>
			<Grid item className={classes.boardArea} xs={12} md={9}>
				<div ref={activeViewContainerRef} className={classes.activeViewContainer}>
					{activeView === ActiveView.Map && activeViewDimensions && (
						<MainView
							game={game}
							width={activeViewDimensions.width}
							height={activeViewDimensions.height}
							showMinimaps={true}
							minimapClicked={view => dispatch(setActiveView(view))}
						/>
					)}
					{activeView === ActiveView.ResearchBoard && activeViewDimensions && (
						<ResearchBoard board={game.boardState.researchBoard} width={activeViewDimensions.width} height={activeViewDimensions.height} />
					)}
					{activeView === ActiveView.ScoringBoard && (
						<ScoringBoard
							board={game.boardState.scoringBoard}
							roundBoosters={game.boardState.availableRoundBoosters}
							federationTokens={game.boardState.availableFederations}
							isMobile={false}
						/>
					)}
					{activeView === ActiveView.PlayerAreas && <PlayerAreas players={players} />}
					{activeView === ActiveView.NotesAndSettings && <PlayerConfig gameId={game.id} />}
				</div>
				<Tabs
					className={classes.tabs}
					value={activeView}
					onChange={(__, val: ActiveView) => dispatch(setActiveView(val))}
					indicatorColor="primary"
					variant={"standard"}
					centered
				>
					<Tab className="gaia-font" label="Map" value={ActiveView.Map} />
					<Tab className="gaia-font" label="Research" value={ActiveView.ResearchBoard} />
					<Tab className="gaia-font" label="Scoring" value={ActiveView.ScoringBoard} />
					<Tab className="gaia-font" label="Players" value={ActiveView.PlayerAreas} />
					<Tab className="gaia-font" label="Notes" value={ActiveView.NotesAndSettings} />
				</Tabs>
			</Grid>
			<Grid item className={classes.controlArea} xs={12} md={3}>
				{PlayerBoxesAndLogs}
			</Grid>
		</Grid>
	);
};

export default DesktopView;
