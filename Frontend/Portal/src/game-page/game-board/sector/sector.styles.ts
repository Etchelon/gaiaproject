import { createStyles, makeStyles } from "@material-ui/core/styles";
import { smartMemoize } from "../../../utils/miscellanea";
import { hexSpike, tileHeight, tileWidth } from "../shape-utils";

interface SectorSizingProps {
	hexWidth: number;
	hexHeight: number;
	xOffset: number;
	yOffset: number;
	row: number;
	column: number;
	rotation: number;
}

const getX = smartMemoize((column: number, hexWidth: number) => {
	const columnWidth = hexWidth - hexSpike(hexWidth);
	return columnWidth * column;
});

const getY = smartMemoize((row: number, hexHeight: number) => {
	const rowHeight = hexHeight / 2;
	return rowHeight * row;
});

const getSectorWidth = smartMemoize((hexWidth: number) => {
	return tileWidth(hexWidth);
});

const getSectorHeight = smartMemoize((hexWidth: number, hexHeight: number) => {
	return tileHeight(hexWidth, hexHeight);
});

const useStyles = makeStyles(() =>
	createStyles({
		sector: {
			position: "absolute",
			width: ({ hexWidth }: SectorSizingProps) => getSectorWidth(hexWidth),
			height: ({ hexWidth, hexHeight }: SectorSizingProps) => getSectorHeight(hexWidth, hexHeight),
			top: ({ yOffset, row, hexHeight }: SectorSizingProps) => getY(row, hexHeight) + yOffset,
			left: ({ xOffset, column, hexWidth }: SectorSizingProps) => getX(column, hexWidth) + xOffset,
			overflow: "hidden",
			pointerEvents: "none",
		},
		image: {
			position: "absolute",
			top: 0,
			left: 0,
			width: "100%",
			height: "100%",
			transform: ({ rotation }: SectorSizingProps) => `rotateZ(-${rotation * 60}deg)`,
		},
	})
);

export default useStyles;
