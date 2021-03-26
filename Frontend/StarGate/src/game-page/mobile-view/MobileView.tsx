import _ from "lodash";
import { useLayoutEffect, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { GamePhase } from "../../dto/enums";
import { ElementSize } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import PlayerConfig from "../config/PlayerConfig";
import PlayerAreas from "../game-board/player-area/PlayerAreas";
import PlayerBox from "../game-board/player-box/PlayerBox";
import Map from "../game-board/map/Map";
import ResearchBoard from "../game-board/research-board/ResearchBoard";
import ScoringBoard from "../game-board/scoring-board/ScoringBoard";
import { GameViewProps } from "../GamePage";
import GameLog from "../logs/GameLog";
import { rollbackGameAtAction } from "../store/actions-thunks";
import { ActiveView } from "../workflows/types";
import useStyles from "./mobile-view.styles";

const MobileView = ({ game, currentPlayerId, players, activeView }: GameViewProps) => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const activeViewContainerRef = useRef<HTMLDivElement>(null);
	const [activeViewDimensions, setActiveViewDimensions] = useState<Nullable<ElementSize>>(null);
	const isGameCreator = game.createdBy.id === currentPlayerId;
	const canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;
	const map = game.boardState.map;

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
		<div ref={activeViewContainerRef}>
			{!_.isNil(activeViewDimensions) && (
				<>
					<div className={classes.boardArea}>
						<Map map={map} width={activeViewDimensions.width} />
					</div>
					<div className={classes.spacer}></div>
					<ResearchBoard board={game.boardState.researchBoard} width={activeViewDimensions.width} />
					<div className={classes.spacer}></div>
					<ScoringBoard
						board={game.boardState.scoringBoard}
						roundBoosters={game.boardState.availableRoundBoosters}
						federationTokens={game.boardState.availableFederations}
						isMobile={true}
					/>
					<div className={classes.spacer}></div>
					{PlayerBoxesAndLogs}
				</>
			)}
		</div>
	);
};

export default MobileView;
