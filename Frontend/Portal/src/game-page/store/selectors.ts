import { HubConnectionState } from "@microsoft/signalr";
import { find, isNil, findIndex, every, sortBy, partition, indexOf, chain, includes } from "lodash";
import { AdvancedTechnologyTileType, StandardTechnologyTileType, ResearchTrackType, RoundBoosterType, FederationTokenType } from "../../dto/enums";
import { isLastRound, Identifier } from "../../utils/miscellanea";
import { InteractiveElementType, InteractiveElementState } from "../workflows/enums";
import { GamePageViewModel } from "./game-page.vm";

export const selectActiveGame = (vm: GamePageViewModel) => vm.gameState;
export const selectActiveGameStatus = (vm: GamePageViewModel) => vm.status;
export const selectActiveView = (vm: GamePageViewModel) => vm.activeView;
export const selectCurrentPlayer = (vm: GamePageViewModel) => find(vm.gameState?.players, p => p.id === vm.currentUserId) ?? null;
export const selectActivePlayer = (vm: GamePageViewModel) => vm.gameState?.activePlayer;
export const selectIsSpectator = (vm: GamePageViewModel) => selectActiveGame(vm) !== null && selectCurrentPlayer(vm) === null;
export const selectPlayers = (vm: GamePageViewModel) => {
	const game = selectActiveGame(vm);
	if (isNil(game)) {
		return [];
	}

	const isSpectator = selectIsSpectator(vm);
	if (isSpectator) {
		return game.players;
	}

	const currentPlayer = selectCurrentPlayer(vm)!;
	const currentPlayerIndex = findIndex(game.players, p => p.id === currentPlayer.id);
	if (currentPlayerIndex === 0) {
		return game.players;
	}

	// Until all players have selected a race and have the state object, keep the initial order
	const allPlayersAreSetup = every(game.players, p => !isNil(p.state));
	if (!allPlayersAreSetup) {
		return game.players;
	}

	// Assume players are already sorted by turn order in the current round
	const allPlayers = sortBy(game.players, p => p.state.currentRoundTurnOrder);
	const [active, havePassed] = partition(allPlayers, p => indexOf(game.players, p) < currentPlayerIndex);
	const playerHasPassed = currentPlayer.state.hasPassed ? havePassed : active;
	const playersToPartition = playerHasPassed ? havePassed : active;
	const [before, fromCurrentPlayer] = partition(playersToPartition, p => indexOf(game.players, p) < currentPlayerIndex);
	return [...fromCurrentPlayer, ...before, ...(playerHasPassed ? active : havePassed)];
};
export const selectSortedPlayers = (vm: GamePageViewModel) => {
	const players = selectPlayers(vm);
	return sortBy(players, (p, index) => p.state?.currentRoundTurnOrder ?? index);
};
export const selectSortedActivePlayers = (vm: GamePageViewModel) => {
	const players = selectPlayers(vm);
	return chain(players)
		.filter(p => !p.state?.hasPassed)
		.sortBy((p, index) => p.state?.currentRoundTurnOrder ?? index)
		.value();
};
export const selectSortedPassedPlayers = (vm: GamePageViewModel) => {
	const players = selectPlayers(vm);
	const isLastRound_ = isLastRound(vm.gameState!);
	return chain(players)
		.filter(p => p.state?.hasPassed ?? false)
		.sortBy((p, index) => (isLastRound_ ? p.state?.currentRoundTurnOrder : p.state?.nextRoundTurnOrder) ?? index)
		.value();
};
export const selectAllRoundBoosters = (vm: GamePageViewModel) => {
	const game = selectActiveGame(vm);
	if (isNil(game)) {
		return [];
	}

	const currentPlayer = selectCurrentPlayer(vm);
	const available = game.boardState.availableRoundBoosters;
	const playersBoosters = chain(game.players)
		.filter(p => !isNil(p.state?.roundBooster))
		.sortBy(p => (p.id === currentPlayer?.id ? 0 : 1))
		.map(p => ({
			id: p.state.roundBooster.id,
			isTaken: true,
			player: {
				id: p.id,
				username: p.username,
				avatarUrl: "",
				raceId: p.raceId,
				raceName: null,
				color: null,
				points: p.state?.points,
				isActive: p.isActive,
				placement: null,
			},
			used: p.state.roundBooster.used,
		}))
		.value();
	return [...playersBoosters, ...available];
};
export const selectStatusMessage = (vm: GamePageViewModel) => vm.statusMessage;
export const selectAvailableCommands = (vm: GamePageViewModel) => vm.availableCommands;
export const selectAvailableActions = (vm: GamePageViewModel) => vm.availableActions;

const selectInteractionState = (type: InteractiveElementType) => (id: Identifier) => (vm: GamePageViewModel, playerId?: string) => {
	if (!isNil(playerId) && playerId !== vm.currentUserId) {
		return { isClickable: false, isSelected: false };
	}
	const interactiveElement = find(vm.interactiveElements, el => el.type === type && el.id === id);
	const interactionState = interactiveElement?.state ?? InteractiveElementState.Disabled;
	const isClickable = interactionState === InteractiveElementState.Enabled;
	const isSelected = interactionState === InteractiveElementState.Selected;
	return { isClickable, isSelected, notes: interactiveElement?.notes };
};
export const selectHexInteractionState = (hexId: string) => selectInteractionState(InteractiveElementType.Hex)(hexId);
export const selectAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) => selectInteractionState(InteractiveElementType.AdvancedTile)(type);
export const selectStandardTileInteractionState = (type: StandardTechnologyTileType) => selectInteractionState(InteractiveElementType.StandardTile)(type);
export const selectOwnStandardTileInteractionState = (type: StandardTechnologyTileType) => selectInteractionState(InteractiveElementType.OwnStandardTile)(type);
export const selectOwnAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) => selectInteractionState(InteractiveElementType.OwnAdvancedTile)(type);
export const selectResearchTrackInteractionState = (type: ResearchTrackType) => selectInteractionState(InteractiveElementType.ResearchStep)(type);
export const selectActionSpaceInteractionState = (elementType: InteractiveElementType) => (type: number) => selectInteractionState(elementType)(type);
export const selectRoundBoosterInteractionState = (elementType: InteractiveElementType) => (type: RoundBoosterType) => selectInteractionState(elementType)(type);
export const selectFederationTokenStackInteractionState = (token: FederationTokenType) => selectInteractionState(InteractiveElementType.FederationToken)(token);
export const selectOwnFederationTokenInteractionState = (type: FederationTokenType) => selectInteractionState(InteractiveElementType.OwnFederationToken)(type);
export const selectIsOnline = (playerId: string) => (vm: GamePageViewModel) => {
	const isCurrentPlayer = playerId === vm.currentUserId;
	return isCurrentPlayer ? vm.hubState.connectionState === HubConnectionState.Connected : includes(vm.hubState.onlineUsers, playerId);
};
