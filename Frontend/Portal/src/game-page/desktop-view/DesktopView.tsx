import { Grid, Tab, Tabs } from "@mui/material";
import { isNil } from "lodash";
import { observer } from "mobx-react";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { GamePhase } from "../../dto/enums";
import { PlayerInGameDto } from "../../dto/interfaces";
import { ElementSize } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import PlayerConfig from "../config/PlayerConfig";
import PlayerArea from "../game-board/player-area/PlayerArea";
import PlayerBox from "../game-board/player-box/PlayerBox";
import ResearchBoard from "../game-board/research-board/ResearchBoard";
import ScoringBoard from "../game-board/scoring-board/ScoringBoard";
import { GameViewProps } from "../GamePage";
import { useGamePageContext } from "../GamePage.context";
import GameLog from "../logs/GameLog";
import MainView from "../main-view/MainView";
import { ActiveView } from "../workflows/types";
import useStyles from "./desktop-view.styles";

const PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO = 1.439;

const DesktopView = ({ game, players, activeView, currentPlayerId, isSpectator }: GameViewProps) => {
	const classes = useStyles();
	const { vm } = useGamePageContext();
	const activeViewContainerRef = useRef<HTMLDivElement>(null);
	const [activeViewDimensions, setActiveViewDimensions] = useState<Nullable<ElementSize>>(null);
	const isGameCreator = game.createdBy.id === currentPlayerId;
	const canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;

	const [playerAreaToShow, setPlayerAreaToShow] = useState<Nullable<PlayerInGameDto>>(null);
	const showPlayerArea = (playerId: string) => {
		if (playerId === playerAreaToShow?.id) {
			hidePlayerArea();
			return;
		}
		const player = players.find(p => p.id === playerId)!;
		setPlayerAreaToShow(player);
	};
	const hidePlayerArea = () => {
		setPlayerAreaToShow(null);
	};
	useEffect(() => {
		if (activeView !== ActiveView.PlayerArea) {
			hidePlayerArea();
			return;
		}
		showPlayerArea(currentPlayerId);
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

	const dialogPlayerAreaDimensions = {
		width: 0,
		height: 0,
		top: 0,
		left: 0,
	};
	if (!isNil(activeViewDimensions)) {
		let dpaHeight = activeViewDimensions.height * 0.95;
		let dpaWidth = dpaHeight * PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		if (dpaWidth > activeViewDimensions.width) {
			dpaWidth = activeViewDimensions.width * 0.95;
			dpaHeight = dpaWidth / PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		}
		dialogPlayerAreaDimensions.width = dpaWidth;
		dialogPlayerAreaDimensions.height = dpaHeight;
		dialogPlayerAreaDimensions.top = (activeViewDimensions.height - dpaHeight) / 2;
		dialogPlayerAreaDimensions.left = (activeViewDimensions.width - dpaWidth) / 2;
	}

	const PlayerBoxesAndLogs = (
		<div className={classes.playerBoxesAndLogs}>
			{players.map((p, index) => (
				<div key={p.id} className={classes.playerBox} onClick={() => showPlayerArea(p.id)}>
					<PlayerBox player={p} index={index + 1} />
				</div>
			))}
			{[...game.gameLogs].reverse().map((log, index) => (
				<div key={index} className={classes.gameLog}>
					<GameLog log={log} canRollback={canRollback} doRollback={actionId => vm.rollbackGameAtAction(game.id, actionId)} />
				</div>
			))}
		</div>
	);

	const actualView = activeView === ActiveView.Map || activeView === ActiveView.PlayerArea ? ActiveView.Map : activeView;

	return (
		<Grid container className={classes.root}>
			<Grid item className={classes.boardArea} xs={12} md={9}>
				<div ref={activeViewContainerRef} className={classes.activeViewContainer}>
					{actualView === ActiveView.Map && activeViewDimensions && (
						<MainView
							game={game}
							width={activeViewDimensions.width}
							height={activeViewDimensions.height}
							showMinimaps={true}
							minimapClicked={view => vm.setActiveView(view)}
						/>
					)}
					{actualView === ActiveView.ResearchBoard && activeViewDimensions && (
						<ResearchBoard board={game.boardState.researchBoard} width={activeViewDimensions.width} height={activeViewDimensions.height} />
					)}
					{actualView === ActiveView.ScoringBoard && (
						<ScoringBoard
							board={game.boardState.scoringBoard}
							roundBoosters={game.boardState.availableRoundBoosters}
							federationTokens={game.boardState.availableFederations}
							isMobile={false}
						/>
					)}
					{actualView === ActiveView.NotesAndSettings && <PlayerConfig gameId={game.id} />}
					{!isNil(playerAreaToShow) && (
						<div
							className={classes.hoveredPlayerArea}
							style={{
								width: dialogPlayerAreaDimensions.width,
								height: dialogPlayerAreaDimensions.height,
								left: dialogPlayerAreaDimensions.left,
								top: dialogPlayerAreaDimensions.top,
							}}
						>
							<PlayerArea player={playerAreaToShow} framed={true} />
						</div>
					)}
				</div>
				<Tabs className={classes.tabs} value={actualView} onChange={(__, val: ActiveView) => vm.setActiveView(val)} indicatorColor="primary" variant={"standard"} centered>
					<Tab className="gaia-font" label="Map" value={ActiveView.Map} />
					<Tab className="gaia-font" label="Research" value={ActiveView.ResearchBoard} />
					<Tab className="gaia-font" label="Scoring" value={ActiveView.ScoringBoard} />
					<Tab className="gaia-font" label="Notes" value={ActiveView.NotesAndSettings} />
				</Tabs>
			</Grid>
			<Grid item className={classes.controlArea} xs={12} md={3}>
				{PlayerBoxesAndLogs}
			</Grid>
		</Grid>
	);
};

export default observer(DesktopView);
