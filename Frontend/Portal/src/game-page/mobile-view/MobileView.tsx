import { isNil } from "lodash";
import { observer } from "mobx-react";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { GamePhase } from "../../dto/enums";
import { TOOLBAR_HEIGHT } from "../../frame/appFrame.styles";
import { ElementSize } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import GameMap from "../game-board/map/Map";
import ResearchBoard from "../game-board/research-board/ResearchBoard";
import ScoringBoard from "../game-board/scoring-board/ScoringBoard";
import { GameViewProps, GAMEVIEW_WRAPPER_ID, STATUSBAR_ID } from "../GamePage";
import { useGamePageContext } from "../GamePage.context";
import GameLog from "../logs/GameLog";
import TurnOrderMinimap from "../main-view/turn-order/TurnOrderMinimap";
import { ActiveView } from "../workflows/types";
import useStyles from "./mobile-view.styles";
import PlayerBoxOrArea from "./PlayerBoxOrArea";

const viewsAnchors = new Map<ActiveView, string>([
	[ActiveView.Map, "map"],
	[ActiveView.ResearchBoard, "researchBoard"],
	[ActiveView.ScoringBoard, "scoringBoard"],
	[ActiveView.PlayerArea, "boxesAndLogs"],
]);

const MobileView = ({ game, players, activeView, currentPlayerId }: GameViewProps) => {
	const classes = useStyles();
	const { vm } = useGamePageContext();
	const activeViewContainerRef = useRef<HTMLDivElement>(null);
	const [activeViewDimensions, setActiveViewDimensions] = useState<Nullable<ElementSize>>(null);
	const isGameCreator = game.createdBy.id === currentPlayerId;
	const canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;
	const map = game.boardState.map;

	useEffect(() => {
		const elementId = viewsAnchors.get(activeView) ?? "";
		const element = document.getElementById(elementId);
		if (!element) {
			return;
		}
		const gameViewWrapper = document.getElementById(GAMEVIEW_WRAPPER_ID)!;
		const statusBar = document.getElementById(STATUSBAR_ID)!;
		const top = element.offsetTop - TOOLBAR_HEIGHT - 3 - statusBar.clientHeight - 3; // 3px spacing below the toolbar and status bar
		gameViewWrapper.scrollTo({ top, behavior: "smooth" });
	}, [activeView]);

	useLayoutEffect(() => {
		if (isNil(activeViewContainerRef.current)) {
			return;
		}

		setActiveViewDimensions({
			width: activeViewContainerRef.current.offsetWidth,
			height: activeViewContainerRef.current.offsetHeight,
		});
	}, [activeViewContainerRef, game]);

	const PlayerBoxesAndLogs = (
		<div id="boxesAndLogs" className={classes.playerBoxesAndLogs}>
			{players.map((p, index) => (
				<div key={p.id} className={classes.playerBox}>
					<PlayerBoxOrArea player={p} index={index + 1} forcePlayerAreaView={p.id === currentPlayerId && activeView === ActiveView.PlayerArea} />
				</div>
			))}
			{[...game.gameLogs].reverse().map((log, index) => (
				<div key={index} className={classes.gameLog}>
					<GameLog log={log} canRollback={canRollback} doRollback={actionId => vm.rollbackGameAtAction(game.id, actionId)} />
				</div>
			))}
		</div>
	);

	return (
		<div ref={activeViewContainerRef}>
			{!isNil(activeViewDimensions) && (
				<>
					<div id="map" className={classes.boardArea}>
						<GameMap map={map} width={activeViewDimensions.width} />
					</div>
					<div className={classes.spacer}></div>
					<div id="researchBoard">
						<ResearchBoard board={game.boardState.researchBoard} width={activeViewDimensions.width} />
					</div>
					<div className={classes.spacer}></div>
					<div id="scoringBoard">
						<ScoringBoard
							board={game.boardState.scoringBoard}
							roundBoosters={game.boardState.availableRoundBoosters}
							federationTokens={game.boardState.availableFederations}
							isMobile={true}
						/>
					</div>
					<div className={classes.spacer}></div>
					<div id="turnOrder">
						<TurnOrderMinimap game={game} direction="horizontal" />
					</div>
					<div className={classes.spacer}></div>
					{PlayerBoxesAndLogs}
				</>
			)}
		</div>
	);
};

export default observer(MobileView);
