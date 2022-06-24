import { HubConnectionState } from "@microsoft/signalr";
import { find, isNil, findIndex, every, sortBy, partition, indexOf, chain, includes } from "lodash";
import {
	AdvancedTechnologyTileType,
	StandardTechnologyTileType,
	ResearchTrackType,
	RoundBoosterType,
	FederationTokenType,
} from "$dto/enums";
import { isLastRound, Identifier } from "$utils/miscellanea";
import type { GameStateDto } from "$dto/interfaces";

export const selectActiveGame = (game: GameStateDto) => game;
export const selectCurrentPlayer = (game: GameStateDto) => game?.players[0]; //find(game?.players, p => p.id === vm.currentUserId) ?? null;
export const selectActivePlayer = (game: GameStateDto) => game?.activePlayer;
export const selectIsSpectator = (game: GameStateDto) => selectActiveGame(game) !== null && selectCurrentPlayer(game) === null;
export const selectPlayers = (game: GameStateDto) => {
	if (!game) {
		return [];
	}

	const isSpectator = selectIsSpectator(game);
	if (isSpectator) {
		return game.players;
	}

	const currentPlayer = selectCurrentPlayer(game)!;
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

export const selectSortedPlayers = (game: GameStateDto) => {
	const players = selectPlayers(game);
	return sortBy(players, (p, index) => p.state?.currentRoundTurnOrder ?? index);
};

export const selectSortedActivePlayers = (game: GameStateDto) => {
	const players = selectPlayers(game);
	return chain(players)
		.filter(p => !p.state?.hasPassed)
		.sortBy((p, index) => p.state?.currentRoundTurnOrder ?? index)
		.value();
};

export const selectSortedPassedPlayers = (game: GameStateDto) => {
	const players = selectPlayers(game);
	const isLastRound_ = isLastRound(game);
	return chain(players)
		.filter(p => p.state?.hasPassed ?? false)
		.sortBy((p, index) => (isLastRound_ ? p.state?.currentRoundTurnOrder : p.state?.nextRoundTurnOrder) ?? index)
		.value();
};

export const selectAllRoundBoosters = (game: GameStateDto) => {
	if (!game) {
		return [];
	}

	const currentPlayer = selectCurrentPlayer(game);
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
