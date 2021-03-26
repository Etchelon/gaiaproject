import { CircularProgress, useMediaQuery } from "@material-ui/core";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import _ from "lodash";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useHistory, useParams } from "react-router-dom";
import { Subscription } from "rxjs";
import { useAssetUrl, useCurrentUser, usePageActivation } from "../utils/hooks";
import { localizeEnum } from "../utils/localization";
import { isMobileOrTablet, Nullable } from "../utils/miscellanea";
import AuctionDialog from "./dialogs/auction/AuctionDialog";
import ConversionsDialog from "./dialogs/conversions/ConversionsDialog";
import SelectRaceDialog from "./dialogs/select-race/SelectRaceDialog";
import SortIncomesDialog from "./dialogs/sort-incomes/SortIncomesDialog";
import TerransDecideIncomeDialog from "./dialogs/terrans-decide-income/TerransDecideIncomeDialog";
import { useGamePageSignalRConnection } from "./game-page-signalr-hook";
import useStyles from "./game-page.styles";
import StatusBar from "./status-bar/StatusBar";
import {
	clearStatus,
	fetchActiveGame,
	selectActiveGame,
	selectActiveGameStatus,
	selectActiveView,
	selectAvailableActions,
	selectCurrentPlayer,
	selectPlayers,
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
import DesktopView from "./desktop-view/DesktopView";
import MobileView from "./mobile-view/MobileView";
import { GameStateDto, PlayerInGameDto } from "../dto/interfaces";

const DIALOG_VIEWS = [ActiveView.RaceSelectionDialog, ActiveView.AuctionDialog, ActiveView.ConversionDialog, ActiveView.SortIncomesDialog, ActiveView.TerransConversionsDialog];
const isDialogView = (view: ActiveView) => _.includes(DIALOG_VIEWS, view);

export interface GameViewProps {
	game: GameStateDto;
	currentPlayerId: string;
	players: PlayerInGameDto[];
	activeView: ActiveView;
}

interface GamePageRouteParams {
	id: string;
}

const GamePage = () => {
	const classes = useStyles();
	const isMobile = useMediaQuery("(max-width: 600px)");
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

	const [activeWorkflow, setActiveWorkflow] = useState<Nullable<ActionWorkflow>>(null);
	const [activeWorkflowSub, setActiveWorkflowSub] = useState<Nullable<Subscription>>(null);

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
	}, [id, status]);

	usePageActivation(connectToHub, () => isMobileOrTablet() && disconnectFromHub(), [id, status]);

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

		if (!isMobile && isActivePlayer) {
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

	if (isLoading || _.isNull(game) || _.isNull(currentPlayer)) {
		return (
			<div className={classes.loader}>
				<CircularProgress />
			</div>
		);
	}

	const activeViewName = localizeEnum(activeView, "ActiveView");

	return (
		<WorkflowContext.Provider key={id} value={{ activeWorkflow, startWorkflow, closeWorkflow }}>
			<div className={classes.root}>
				<div className={classes.statusBar + (isMobile ? " mobile" : " desktop")}>
					<StatusBar game={game} playerId={currentPlayer.id} />
				</div>
				<div className={classes.gameView + (isMobile ? " mobile" : " desktop")}>
					{!isMobile && <DesktopView game={game} currentPlayerId={currentPlayer.id} players={players} activeView={activeView} />}
					{isMobile && <MobileView game={game} currentPlayerId={currentPlayer.id} players={players} activeView={activeView} />}
				</div>
			</div>
			<Dialog
				aria-labelledby="dialog-title"
				fullScreen={isMobile}
				style={isMobile ? undefined : { maxHeight: "90vh", top: "5vh" }}
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
