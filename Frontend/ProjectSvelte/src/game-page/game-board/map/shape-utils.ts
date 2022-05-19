// tslint:disable:max-classes-per-file

//#region Hex

export const hexWidth = (height: number) => height / Math.cos(Math.PI / 6);
export const hexHeight = (width: number) => width * Math.cos(Math.PI / 6);
export const hexHalfHeight = (width: number) => hexHeight(width) / 2;
export const hexSide = (width: number) => width * Math.sin(Math.PI / 6);
export const hexSpike = (width: number) => (width - hexSide(width)) / 2;
// tslint:disable-next-line:no-shadowed-variable
export const tileWidth = (hexWidth: number) => 3 * hexWidth + 2 * hexSide(hexWidth);
// tslint:disable-next-line:no-shadowed-variable
export const tileHeight = (hexWidth: number, height?: number) => 5 * (height ?? hexHeight(hexWidth));
export class Hex {
	width: number;
	height: number;
	halfHeight: number;
	side: number;
	spike: number;
	tileWidth: number;
	tileHeight: number;

	constructor(width: number) {
		this.width = width;
		this.height = hexHeight(width);
		this.halfHeight = hexHalfHeight(width);
		this.side = hexSide(width);
		this.spike = hexSpike(width);
		this.tileWidth = tileWidth(width);
		this.tileHeight = tileHeight(width);
	}
}

//#endregion

//#region Octagon

export const octagonSide = (width: number) => width * 0.41;
export const octagonEmptySide = (width: number) => width * 0.59;
export const octagonHalfEmptySide = (width: number) => width * 0.295;
export class Octagon {
	width: number;
	side: number;
	diameter: number;
	emptySide: number;

	constructor(width: number) {
		this.width = width;
		this.side = octagonSide(width);
		this.diameter = 1.08 * width;
		this.emptySide = width * 0.59;
	}
}
//#endregion

export const rowsInSector = 9;
export const columnsInSector = 5;
export const rotationOffsetsHalfSequences = {
	0: [0, 0, 0, 0, 0, 0, 0, 0, 0],
	1: [5, 1, 8, -3, 3, 10, -2, 5, -7],
	2: [15, 9, 15, 2, 8, 13, 1, 7, 6],
	3: [18, 16, 14, 12, 10, 8, 6, 4, 2],
	4: [13, 15, 6, 15, 7, -2, 8, -1, 9],
	5: [3, 7, -1, 10, 2, -5, 5, -3, 8],
};
