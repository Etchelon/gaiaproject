import { CircularProgress, Grid, Tab, Tabs, useMediaQuery } from "@material-ui/core";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import AccountBoxIcon from "@material-ui/icons/AccountBox";
import FormatAlignCenterIcon from "@material-ui/icons/FormatAlignCenter";
import MapIcon from "@material-ui/icons/Map";
import StarIcon from "@material-ui/icons/Star";
import SupervisedUserCircleIcon from "@material-ui/icons/SupervisedUserCircle";
import _ from "lodash";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useHistory, useParams } from "react-router-dom";
import { Subscription } from "rxjs";
import { GamePhase } from "../dto/enums";
import { PlayerInGameDto } from "../dto/interfaces";
import { ElementSize, useAssetUrl, useCurrentUser } from "../utils/hooks";
import { localizeEnum } from "../utils/localization";
import { isMobileOrTablet, Nullable } from "../utils/miscellanea";
import AuctionDialog from "./dialogs/auction/AuctionDialog";
import ConversionsDialog from "./dialogs/conversions/ConversionsDialog";
import SelectRaceDialog from "./dialogs/select-race/SelectRaceDialog";
import SortIncomesDialog from "./dialogs/sort-incomes/SortIncomesDialog";
import TerransDecideIncomeDialog from "./dialogs/terrans-decide-income/TerransDecideIncomeDialog";
import PlayerArea from "./game-board/player-area/PlayerArea";
import PlayerAreas from "./game-board/player-area/PlayerAreas";
import PlayerBox from "./game-board/player-box/PlayerBox";
import ResearchBoard from "./game-board/research-board/ResearchBoard";
import ScoringBoard from "./game-board/scoring-board/ScoringBoard";
import { useGamePageSignalRConnection } from "./game-page-signalr-hook";
import useStyles from "./game-page.styles";
import GameLog from "./logs/GameLog";
import MainView from "./main-view/MainView";
import StatusBar from "./status-bar/StatusBar";
import {
	clearStatus,
	fetchActiveGame,
	rollbackGameAtAction,
	selectActiveGame,
	selectActiveGameStatus,
	selectActiveView,
	selectAvailableActions,
	selectCurrentPlayer,
	selectPlayers,
	setActiveView,
	setCurrentUser,
	setStatusFromWorkflow,
	setStatusMessage,
	setWaitingForAction,
	unloadActiveGame,
} from "./store/active-game.slice";
import { WorkflowContext } from "./WorkflowContext";
import { ActionWorkflow } from "./workflows/action-workflow.base";
import { ActiveView } from "./workflows/types";
import { fromAction, fromDecision } from "./workflows/utils";

const DIALOG_VIEWS = [ActiveView.RaceSelectionDialog, ActiveView.AuctionDialog, ActiveView.ConversionDialog, ActiveView.SortIncomesDialog, ActiveView.TerransConversionsDialog];
const isDialogView = (view: ActiveView) => _.includes(DIALOG_VIEWS, view);
const closeHoverTimeout: any = { value: undefined };
const PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO = 1.439;

interface GamePageRouteParams {
	id: string;
}

const GamePage = () => {
	const classes = useStyles();
	const useMobileLayout = useMediaQuery("(max-width: 600px)");
	const { id } = useParams<GamePageRouteParams>();
	const { push } = useHistory();
	const dispatch = useDispatch();
	const playersTurnAudioUrl = useAssetUrl("Sounds/PlayersTurn.wav");
	const { connectToHub, disconnectFromHub } = useGamePageSignalRConnection(id, closeWorkflow);

	const user = useCurrentUser();
	const game = useSelector(selectActiveGame);
	const players = useSelector(selectPlayers);
	const currentPlayer = useSelector(selectCurrentPlayer);
	const status = useSelector(selectActiveGameStatus);
	const availableActions = useSelector(selectAvailableActions);
	const isLoading = status === "loading";
	const activeView = useSelector(selectActiveView);

	const activeViewContainerRef = useRef<HTMLDivElement>(null);
	const [activeViewDimensions, setActiveViewDimensions] = useState<Nullable<ElementSize>>(null);
	const [activeWorkflow, setActiveWorkflow] = useState<Nullable<ActionWorkflow>>(null);
	const [activeWorkflowSub, setActiveWorkflowSub] = useState<Nullable<Subscription>>(null);
	const [hoveredPlayer, setHoveredPlayer] = useState<Nullable<PlayerInGameDto>>(null);
	window.clearTimeout(closeHoverTimeout.value);

	const showPlayerArea = (player: PlayerInGameDto) => {
		if (!_.isNil(hoveredPlayer)) {
			return;
		}
		setHoveredPlayer(player);
	};

	const hidePlayerArea = () => {
		setHoveredPlayer(null);
	};

	useEffect(() => {
		document.addEventListener("keydown", hidePlayerArea, false);
		return () => {
			document.removeEventListener("keydown", hidePlayerArea);
		};
	}, []);

	const showDialog = isDialogView(activeView);

	//#region Setup workflow context

	function closeWorkflow(): void {
		activeWorkflowSub?.unsubscribe();
		setActiveWorkflowSub(null);
		setActiveWorkflow(null);
	}

	const startWorkflow = (workflow: ActionWorkflow): void => {
		const sub = workflow.currentState$.subscribe(state => {
			dispatch(setStatusFromWorkflow(state));
		});
		sub.add(
			workflow.switchToAction$.subscribe(actionType => {
				closeWorkflow();

				if (_.isNil(actionType)) {
					dispatch(setWaitingForAction());
					return;
				}

				const action = _.find(availableActions, act => act.type === actionType)!;
				const newWorkflow = fromAction(currentPlayer!.id, game!, action);
				startWorkflow(newWorkflow);
			})
		);
		setActiveWorkflow(workflow);
		setActiveWorkflowSub(sub);
	};

	//#endregion

	//#region onInit: load the game

	useEffect(() => {
		dispatch(fetchActiveGame(id));

		return () => {
			closeWorkflow();
			dispatch(unloadActiveGame());
			disconnectFromHub();
		};
	}, []);

	//#endregion

	useEffect(() => {
		user && dispatch(setCurrentUser(user.id));
	}, [user]);

	//#region When the game has loaded connect to SignalR hub

	useEffect(() => {
		if (status === "failure") {
			console.error(`Failed to initialize Active Game ${id}.`);
			push("/not-found");
			return;
		}

		if (status !== "success") {
			return;
		}

		connectToHub();

		// Setup hub management
		const handleTabActivationChange = (isActive: boolean) => {
			if (isActive) {
				connectToHub();
			} else {
				isMobileOrTablet() && disconnectFromHub();
			}
		};
		const onVisibilityChange = () => {
			const isVisible = document.visibilityState === "visible";
			handleTabActivationChange(isVisible);
		};
		document.addEventListener("visibilitychange", onVisibilityChange);
		const onPageShow = () => {
			handleTabActivationChange(true);
		};
		window.addEventListener("pageshow", onPageShow);
		const onPageHide = () => {
			handleTabActivationChange(false);
		};
		window.addEventListener("pagehide", onPageHide);

		return () => {
			document.removeEventListener("visibilitychange", onVisibilityChange);
			window.removeEventListener("pageshow", onPageShow);
			window.removeEventListener("pagehide", onPageHide);
		};
	}, [id, status]);

	//#endregion

	//#region When game has loaded check for the player's state

	useEffect(() => {
		if (_.isNull(game) || _.isNull(currentPlayer)) {
			return;
		}

		const gameIsOver = !_.isNil(game.ended);
		if (gameIsOver) {
			const winnersNames = _.chain(game.players)
				.filter(p => p.placement === 1)
				.map(p => p.username)
				.value();
			const message = `Game over. ${winnersNames.join(", ")} won!`;
			dispatch(setStatusMessage(message));
			return;
		}

		const activePlayer = game.activePlayer;
		if (!activePlayer) {
			dispatch(clearStatus());
			return;
		}

		const isActivePlayer = activePlayer.id === currentPlayer.id;
		if (!isActivePlayer) {
			setActiveWorkflow(null);
			dispatch(setStatusMessage(activePlayer.reason));
			return;
		}

		if (!useMobileLayout && isActivePlayer) {
			const audio = new Audio(playersTurnAudioUrl);
			audio.volume = 0.5;
			audio.play();
		}
		let workflow: ActionWorkflow | null = null;
		if (activePlayer.pendingDecision) {
			workflow = fromDecision(currentPlayer.id, game, activePlayer.pendingDecision);
		} else if (_.size(activePlayer.availableActions) === 1) {
			workflow = fromAction(currentPlayer.id, game, activePlayer.availableActions[0]);
		}

		if (!workflow) {
			dispatch(setWaitingForAction(activePlayer.availableActions));
			return;
		}

		startWorkflow(workflow);

		return () => {
			closeWorkflow();
		};
	}, [user, game, currentPlayer]);

	//#endregion

	//#region Render synchronously with the correct dimensions

	useLayoutEffect(() => {
		if (_.isNil(activeViewContainerRef.current)) {
			return;
		}

		setActiveViewDimensions({
			width: activeViewContainerRef.current.offsetWidth,
			height: activeViewContainerRef.current.offsetHeight,
		});
	}, [activeViewContainerRef, game, currentPlayer, user]);

	//#endregion

	if (isLoading || _.isNull(game) || _.isNull(currentPlayer)) {
		return (
			<div className={classes.loader}>
				<CircularProgress />
			</div>
		);
	}

	const activeViewName = localizeEnum(activeView, "ActiveView");
	const isGameCreator = game.createdBy.id === currentPlayer.id;
	const canRollback = isGameCreator && game.currentPhase === GamePhase.Rounds;

	const hoveredPlayerAreaDimensions = {
		width: 0,
		height: 0,
		top: 0,
		left: 0,
	};
	if (!_.isNil(activeViewDimensions)) {
		let hpaHeight = activeViewDimensions.height * 0.9;
		let hpaWidth = hpaHeight * PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		if (hpaWidth > activeViewDimensions.width) {
			hpaWidth = activeViewDimensions.width * 0.9;
			hpaHeight = hpaWidth / PLAYER_AREA_WIDTH_TO_HEIGHT_RATIO;
		}
		hoveredPlayerAreaDimensions.width = hpaWidth;
		hoveredPlayerAreaDimensions.height = hpaHeight;
		hoveredPlayerAreaDimensions.top = (activeViewDimensions.height - hpaHeight) / 2;
		hoveredPlayerAreaDimensions.left = (activeViewDimensions.width - hpaWidth) / 2;
	}

	const PlayerBoxesAndLogs = (
		<div className={classes.playerBoxesAndLogs}>
			{_.map(players, (p, index) => (
				<div key={p.id} className={classes.playerBox}>
					<PlayerBox player={p} index={index + 1} />
					<div className="hoverTrap" onMouseEnter={() => showPlayerArea(p)} onMouseLeave={() => (closeHoverTimeout.value = window.setTimeout(hidePlayerArea, 250))}></div>
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
		<WorkflowContext.Provider key={id} value={{ activeWorkflow, startWorkflow, closeWorkflow }}>
			<div key={game.id} className={classes.root + " game-page"}>
				<div className={classes.statusBar}>
					<StatusBar game={game} playerId={currentPlayer.id} />
				</div>
				<Grid container className={classes.wrapper}>
					<Grid item className={classes.boardArea} xs={12} md={9}>
						<div ref={activeViewContainerRef} className={classes.activeViewContainer}>
							{activeView === ActiveView.Map && activeViewDimensions && (
								<MainView
									game={game}
									width={activeViewDimensions.width}
									height={activeViewDimensions.height}
									showMinimaps={!useMobileLayout}
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
								/>
							)}
							{activeView === ActiveView.PlayerAreas && <PlayerAreas players={players} />}
							{!useMobileLayout && !_.isNil(hoveredPlayer) && (
								<div
									className={classes.hoveredPlayerArea}
									style={{
										width: hoveredPlayerAreaDimensions.width,
										height: hoveredPlayerAreaDimensions.height,
										left: hoveredPlayerAreaDimensions.left,
										top: hoveredPlayerAreaDimensions.top,
									}}
								>
									<PlayerArea player={hoveredPlayer} framed={true} />
								</div>
							)}
							{useMobileLayout && activeView === ActiveView.MobilePlayerBoxes && PlayerBoxesAndLogs}
						</div>
						<Tabs
							className={classes.tabs}
							value={showDialog ? ActiveView.Map : activeView}
							onChange={(__, val: ActiveView) => dispatch(setActiveView(val))}
							indicatorColor="primary"
							variant={useMobileLayout ? "fullWidth" : "standard"}
							centered={!useMobileLayout}
						>
							<Tab className="gaia-font" label={useMobileLayout ? "" : "Map"} icon={useMobileLayout ? <MapIcon /> : ""} value={ActiveView.Map} />
							<Tab
								className="gaia-font"
								label={useMobileLayout ? "" : "Research"}
								icon={useMobileLayout ? <FormatAlignCenterIcon /> : ""}
								value={ActiveView.ResearchBoard}
							/>
							<Tab className="gaia-font" label={useMobileLayout ? "" : "Scoring"} icon={useMobileLayout ? <StarIcon /> : ""} value={ActiveView.ScoringBoard} />
							<Tab
								className="gaia-font"
								label={useMobileLayout ? "" : "Players"}
								icon={useMobileLayout ? <SupervisedUserCircleIcon /> : ""}
								value={ActiveView.PlayerAreas}
							/>
							{useMobileLayout && <Tab className="gaia-font" label="" icon={<AccountBoxIcon />} value={ActiveView.MobilePlayerBoxes} />}
						</Tabs>
					</Grid>
					{!useMobileLayout && (
						<Grid item className={classes.controlArea} xs={12} md={3}>
							{PlayerBoxesAndLogs}
						</Grid>
					)}
				</Grid>
			</div>

			<Dialog
				aria-labelledby="dialog-title"
				fullScreen={useMobileLayout}
				style={useMobileLayout ? undefined : { maxHeight: "90vh", top: "5vh" }}
				open={showDialog}
				maxWidth={"lg"}
				fullWidth={false}
				disableBackdropClick={true}
				keepMounted={false}
			>
				{activeViewName && (
					<DialogTitle id="dialog-title">
						<div className="gaia-font text-center">{activeViewName}</div>
					</DialogTitle>
				)}
				{activeView === ActiveView.RaceSelectionDialog && <SelectRaceDialog gameId={game.id} currentPlayer={currentPlayer} />}
				{activeView === ActiveView.AuctionDialog && <AuctionDialog gameId={game.id} currentPlayer={currentPlayer} />}
				{activeView === ActiveView.ConversionDialog && <ConversionsDialog gameId={game.id} currentPlayer={currentPlayer} />}
				{activeView === ActiveView.SortIncomesDialog && <SortIncomesDialog gameId={game.id} currentPlayer={currentPlayer} />}
				{activeView === ActiveView.TerransConversionsDialog && <TerransDecideIncomeDialog gameId={game.id} currentPlayer={currentPlayer} />}
			</Dialog>
		</WorkflowContext.Provider>
	);
};

export default GamePage;
