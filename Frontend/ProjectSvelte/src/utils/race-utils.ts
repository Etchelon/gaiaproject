import type { Race } from "../dto/enums";
import { getContrastColor } from "./colors";
import type { Nullable } from "./miscellanea";

const raceNames = new Map<Race, string>([
	[1, "Terrans"],
	[2, "Lantids"],
	[3, "Taklons"],
	[4, "Ambas"],
	[5, "Gleens"],
	[6, "Xenos"],
	[7, "Ivits"],
	[8, "HadschHallas"],
	[9, "Bescods"],
	[10, "Firaks"],
	[11, "Geodens"],
	[12, "BalTaks"],
	[13, "Nevlas"],
	[14, "Itars"],
]);

const raceColors = new Map<Race, string>([
	[1, "#02509E"],
	[2, "#02509E"],
	[3, "#7E532E"],
	[4, "#7E532E"],
	[5, "#FFCC00"],
	[6, "#FFCC00"],
	[7, "#D21318"],
	[8, "#D21318"],
	[9, "#909399"],
	[10, "#909399"],
	[11, "#EC6706"],
	[12, "#EC6706"],
	[13, "#FFFFFF"],
	[14, "#FFFFFF"],
]);

export function getRaceName(raceId: Race | null): string {
	return raceId === null ? "" : raceNames.get(raceId)!;
}

export function getRaceColor(raceId: Nullable<Race>): string {
	return raceId === null ? "#000" : raceColors.get(raceId)!;
}

export function getRaceColors(raceId: Nullable<Race>): [string, string] {
	const color = raceId === null ? "#000" : raceColors.get(raceId)!;
	const contrast = getContrastColor(color);
	return [color, contrast];
}

export function getRaceImage(raceId: Nullable<Race>): string {
	return raceId === null ? "" : getRaceName(raceId) + ".png";
}

export function getRaceBoard(raceId: Nullable<Race>): Nullable<string> {
	return raceId === null ? null : getRaceName(raceId) + ".jpg";
}
