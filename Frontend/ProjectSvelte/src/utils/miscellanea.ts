import { chain, delay, isNil } from "lodash";
import { DateTime } from "luxon";
import { AdvancedTechnologyTileType, GamePhase, RoundBoosterType, StandardTechnologyTileType } from "../dto/enums";
import type { GameStateDto, HexDto, PlayerInfoDto, PlayerInGameDto } from "../dto/interfaces";

const LAST_ROUND = 6;
export const CENTRAL_HEX_INDEX = 9;

export function isAuctionOngoing(game: GameStateDto): boolean {
	if (!game.auctionState) {
		return false;
	}
	if (game.currentPhase !== GamePhase.Setup) {
		return false;
	}

	return !game.auctionState.auctionedRaces.every(o => o.playerUsername !== null);
}

export function isLastRound(game: GameStateDto): boolean {
	return game.currentRound === LAST_ROUND;
}

export const executeWhen = (action: () => void, predicate: () => boolean, pollingTimeMs = 10): void => {
	if (predicate()) {
		action();
		return;
	}

	delay(() => executeWhen(action, predicate), pollingTimeMs);
};

export const asyncDelay = (timeoutMs: number) =>
	new Promise<void>(resolve => {
		setTimeout(resolve, timeoutMs);
	});

export interface Point {
	x: number;
	y: number;
}

export const interactiveElementClass = (isClickable: boolean, isSelected: boolean, rootClass = "") =>
	`interactive-element${isSelected ? " selected" : isClickable ? " clickable" : ""}${rootClass ? ` ${rootClass}` : ""}`;

export const withAspectRatioW = (wTohRatio: number) => `
	position: relative;
	width: 100%;
	padding-top: ${100 / wTohRatio}%
`;

export const withAspectRatioH = (hTowRatio: number) => `
	position: relative;
	height: 100%;
	padding-left: ${100 / hTowRatio}%
`;

export function getHex(id: string, game: GameStateDto): HexDto {
	return chain(game.boardState.map.sectors)
		.flatMap(s => s.hexes)
		.find(h => h.id === id)
		.value();
}

export type Identifier = number | string;

export type Nullable<T> = T | null;

export function prettyTimestamp(isoDate: string): string {
	const date = DateTime.fromISO(isoDate);
	const now = DateTime.now();
	return date.diff(now).toHuman();
}

export type UniversalFn<TResult = any> = (...args: any[]) => TResult;

const ACTIVATABLE_ROUND_BOOSTERS: RoundBoosterType[] = [RoundBoosterType.BoostRangeGainPower, RoundBoosterType.TerraformActionGainCredits];

const ACTIVATABLE_ADVANCED_TILES: AdvancedTechnologyTileType[] = [
	AdvancedTechnologyTileType.ActionGain1Qic5Credits,
	AdvancedTechnologyTileType.ActionGain3Knowledge,
	AdvancedTechnologyTileType.ActionGain3Ores,
];

export const countActivatableActions = (player: PlayerInGameDto, includeGaiaformers: boolean) => {
	const ps = player.state;
	let allSpecialActionsCount = 0;
	let availableSpecialActionsCount = 0;

	if (player.raceId === null) {
		return { available: 0, all: 0 };
	}

	if (!isNil(ps.planetaryInstituteActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.planetaryInstituteActionSpace.isAvailable);
	}
	if (!isNil(ps.rightAcademyActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.rightAcademyActionSpace.isAvailable);
	}
	if (!isNil(ps.raceActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.raceActionSpace.isAvailable);
	}

	if (ACTIVATABLE_ROUND_BOOSTERS.includes(ps.roundBooster?.id)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(!ps.roundBooster.used);
	}

	const gain4PowerTile = ps.technologyTiles.find(
		tt => tt.id === StandardTechnologyTileType.ActionGain4Power && isNil(tt.coveredByAdvancedTile)
	);
	if (gain4PowerTile) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(!gain4PowerTile.used);
	}

	chain(ps.technologyTiles)
		.filter(tt => !isNil(tt.coveredByAdvancedTile) && ACTIVATABLE_ADVANCED_TILES.includes(tt.coveredByAdvancedTile))
		.each(tt => {
			allSpecialActionsCount += 1;
			availableSpecialActionsCount += Number(!tt.used);
		});

	if (includeGaiaformers) {
		allSpecialActionsCount += ps.unlockedGaiaformers;
		availableSpecialActionsCount += ps.usableGaiaformers;
	}

	return { available: availableSpecialActionsCount, all: allSpecialActionsCount };
};

export const assetUrl = (relativeUrl: string) => `/assets/Resources/${relativeUrl}`;

export const playerInitials = (player: PlayerInfoDto) => player.username.substring(0, 2);
