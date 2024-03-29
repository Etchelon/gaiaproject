import { Theme } from "@mui/material";
import { formatDistance } from "date-fns";
import _ from "lodash";
import { ActionType, AdvancedTechnologyTileType, GamePhase, RoundBoosterType, StandardTechnologyTileType } from "../dto/enums";
import { GameStateDto, HexDto, PlayerInGameDto } from "../dto/interfaces";

const LAST_ROUND = 6;
export const CENTRAL_HEX_INDEX = 9;

export function isAuctionOngoing(game: GameStateDto): boolean {
	if (!game.auctionState) {
		return false;
	}
	if (game.currentPhase !== GamePhase.Setup) {
		return false;
	}

	return !_.every(game.auctionState.auctionedRaces, o => o.playerUsername !== null);
}

export function isLastRound(game: GameStateDto): boolean {
	return game.currentRound === LAST_ROUND;
}

export const executeWhen = (action: () => void, predicate: () => boolean, pollingTimeMs = 10): void => {
	if (predicate()) {
		action();
		return;
	}

	_.delay(() => executeWhen(action, predicate), pollingTimeMs);
};

export interface Point {
	x: number;
	y: number;
}

export const flexRow = {
	display: "flex",
	alignItems: "center",
};

export const centeredFlexRow = {
	...flexRow,
	justifyContent: "center",
};

export const centeredFlexColumn = {
	display: "flex",
	flexFlow: "column",
	alignItems: "center",
};

export const fillParent = {
	width: "100%",
	height: "100%",
};

export const fillParentAbs: any = {
	...fillParent,
	position: "absolute",
	top: 0,
	left: 0,
};

export const fillWindow = {
	width: "100vw",
	height: "100vh",
};

export const interactiveBorder: any = {
	borderWidth: 3,
	borderStyle: "solid",
	borderColor: "transparent",
	"&.clickable": {
		borderColor: "dodgerblue",
	},
	"&.selected": {
		borderColor: "red",
	},
};

export const withAspectRatioW = (wTohRatio: number): any => ({
	position: "relative",
	width: "100%",
	paddingTop: `${100 / wTohRatio}%`,
});

export const withAspectRatioH = (hTowRatio: number): any => ({
	position: "relative",
	height: "100%",
	paddingLeft: `${100 / hTowRatio}%`,
});

// For the following smartMemoize implementation, see https://dev.to/nioufe/you-should-not-use-lodash-for-memoization-3441
const hasher = (...args: any[]): string => JSON.stringify(args);
export const smartMemoize = (fn: (...args: any[]) => any) => _.memoize(fn, hasher);

export type Identifier = number | string;

export function getHex(id: string, game: GameStateDto): HexDto {
	return _.chain(game.boardState.map.sectors)
		.flatMap(s => s.hexes)
		.find(h => h.id === id)
		.value();
}

export const interactiveElementClass = (isClickable: boolean, isSelected: boolean, rootClass = "") =>
	`${rootClass} interactive${isSelected ? " selected" : isClickable ? " clickable" : ""}`;

export type Nullable<T> = T | null;

export const responsivePadding = (theme: Theme) => {
	return {
		padding: theme.spacing(3),
		[theme.breakpoints.down("md")]: {
			padding: theme.spacing(2),
		},
		[theme.breakpoints.down("sm")]: {
			padding: theme.spacing(1),
		},
	};
};

export function isMobileOrTablet() {
	let check = false;
	// tslint:disable-next-line:only-arrow-functions
	(function (a) {
		// tslint:disable-next-line:curly
		if (
			/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(
				a
			) ||
			/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw-(n|u)|c55\/|capi|ccwa|cdm-|cell|chtm|cldc|cmd-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc-s|devi|dica|dmob|do(c|p)o|ds(12|-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(-|_)|g1 u|g560|gene|gf-5|g-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd-(m|p|t)|hei-|hi(pt|ta)|hp( i|ip)|hs-c|ht(c(-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i-(20|go|ma)|i230|iac( |-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|-[a-w])|libw|lynx|m1-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|-([1-8]|c))|phil|pire|pl(ay|uc)|pn-2|po(ck|rt|se)|prox|psio|pt-g|qa-a|qc(07|12|21|32|60|-[2-7]|i-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h-|oo|p-)|sdk\/|se(c(-|0|1)|47|mc|nd|ri)|sgh-|shar|sie(-|m)|sk-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h-|v-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl-|tdg-|tel(i|m)|tim-|t-mo|to(pl|sh)|ts(70|m-|m3|m5)|tx-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas-|your|zeto|zte-/i.test(
				a.substr(0, 4)
			)
		) {
			check = true;
		}
	})(navigator.userAgent || navigator.vendor || (window as any).opera);
	return check;
}

export function prettyTimestamp(isoDate: string): string {
	const date = Date.parse(isoDate);
	const now = Date.now();
	return formatDistance(date, now, { addSuffix: true });
}

export type UniversalFn<TResult = any> = (...args: any[]) => TResult;

const ACTIVATABLE_ACTIONS: ActionType[] = [
	ActionType.AmbasSwapPlanetaryInstituteAndMine,
	ActionType.BescodsResearchProgress,
	ActionType.FiraksDowngradeResearchLab,
	ActionType.IvitsPlaceSpaceStation,
	ActionType.StartGaiaProject,
	ActionType.UseRightAcademy,
	ActionType.UseRoundBooster,
	ActionType.UseTechnologyTile,
];

const ACTIVATABLE_ROUND_BOOSTERS: RoundBoosterType[] = [RoundBoosterType.BoostRangeGainPower, RoundBoosterType.TerraformActionGainCredits];

const ACTIVATABLE_ADVANCED_TILES: AdvancedTechnologyTileType[] = [
	AdvancedTechnologyTileType.ActionGain1Qic5Credits,
	AdvancedTechnologyTileType.ActionGain3Knowledge,
	AdvancedTechnologyTileType.ActionGain3Ores,
];

const ACTIVATABLE_STANDARD_TILES: StandardTechnologyTileType[] = [StandardTechnologyTileType.ActionGain4Power];

export const countActivatableActions = (player: PlayerInGameDto, includeGaiaformers: boolean) => {
	const ps = player.state;
	let allSpecialActionsCount = 0;
	let availableSpecialActionsCount = 0;

	if (player.raceId === null) {
		return { available: 0, all: 0 };
	}

	if (!_.isNil(ps.planetaryInstituteActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.planetaryInstituteActionSpace.isAvailable);
	}
	if (!_.isNil(ps.rightAcademyActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.rightAcademyActionSpace.isAvailable);
	}
	if (!_.isNil(ps.raceActionSpace)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(ps.raceActionSpace.isAvailable);
	}

	if (ACTIVATABLE_ROUND_BOOSTERS.includes(ps.roundBooster?.id)) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(!ps.roundBooster.used);
	}

	const gain4PowerTile = _.find(ps.technologyTiles, tt => tt.id === StandardTechnologyTileType.ActionGain4Power && _.isNil(tt.coveredByAdvancedTile));
	if (gain4PowerTile) {
		allSpecialActionsCount += 1;
		availableSpecialActionsCount += Number(!gain4PowerTile.used);
	}

	_.chain(ps.technologyTiles)
		.filter(tt => !_.isNil(tt.coveredByAdvancedTile) && ACTIVATABLE_ADVANCED_TILES.includes(tt.coveredByAdvancedTile))
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
