import { createStyles, makeStyles } from "@material-ui/core/styles";
import { smartMemoize } from "../../../utils/miscellanea";

interface HexSizingProps {
	index: number;
	width: number;
	height: number;
	side: number;
}

const getRowAndColumn = smartMemoize((index: number) => {
	const indexMinus3 = index - 3;
	const normalizedTo5 = indexMinus3 % 5;
	const row =
		3 <= index && index <= 15
			? 2 + (normalizedTo5 / 2 <= 1 ? 0 : 1) + 2 * parseInt(String((index - 3) / 5), 10) // replace with Math.floor?
			: index === 0
			? 0
			: index === 18
			? 8
			: index === 1 || index === 2
			? 1
			: index === 16 || index === 17
			? 7
			: 0;
	const column =
		3 <= index && index <= 15
			? normalizedTo5 < 3
				? 2 * (normalizedTo5 % 3)
				: 3 - 2 * (normalizedTo5 % 2)
			: index === 0 || index === 18
			? 2
			: index === 1 || index === 16
			? 1
			: index === 2 || index === 17
			? 3
			: 0;
	return { row, column };
});

const getX = smartMemoize((index: number, width: number, side: number) => {
	const { row, column } = getRowAndColumn(index);
	const isEvenRow = row % 2 === 0;
	return isEvenRow ? (column / 2) * (width + side) : (width + side) / 2 + ((column - 1) / 2) * (side + width);
});

const getY = smartMemoize((index: number, height: number) => {
	const { row } = getRowAndColumn(index);
	return (height / 2) * row;
});

const useStyles = makeStyles(() =>
	createStyles({
		hex: {
			position: "absolute",
			width: ({ width }: HexSizingProps) => width,
			height: ({ height }: HexSizingProps) => height,
			top: ({ index, height }: HexSizingProps) => getY(index, height),
			left: ({ index, width, side }: HexSizingProps) => getX(index, width, side),
			pointerEvents: "none",
		},
		gaiaMarker: {
			position: "absolute",
			width: ({ width }: HexSizingProps) => width * 0.75,
			height: ({ width }: HexSizingProps) => width * 0.75,
			top: ({ height }: HexSizingProps) => (height * 0.25) / 2,
			left: ({ width }: HexSizingProps) => (width * 0.25) / 2,
		},
		building: {
			position: "absolute",
			zIndex: 1,
		},
		federationMarker: {
			position: "absolute",
			width: ({ width }: HexSizingProps) => width * 0.35,
			height: ({ width }: HexSizingProps) => width * 0.35,
			bottom: ({ height }: HexSizingProps) => (height * 0.3) / 2,
			left: ({ width }: HexSizingProps) => (width * 0.3) / 2,
		},
		lantidsMine: {
			position: "absolute",
			top: ({ height }: HexSizingProps) => (height * 0.25) / 2,
			left: ({ width }: HexSizingProps) => width * 0.25,
			zIndex: 2,
		},
		satellites: {
			position: "absolute",
			width: "100%",
			height: "100%",
			padding: "15%",
			display: "flex",
			flexFlow: "row wrap",
			alignItems: "center",
			justifyContent: "space-around",
		},
		selector: {
			position: "absolute",
			width: ({ height }: HexSizingProps) => height * 0.95,
			height: ({ height }: HexSizingProps) => height * 0.95,
			top: ({ height }: HexSizingProps) => (height * 0.05) / 2,
			left: ({ width, height }: HexSizingProps) => (width - height * 0.95) / 2,
			border: "2px solid dodgerblue",
			borderRadius: "50%",
			"&.selected": {
				borderColor: "red",
			},
			"&.with-qic": {
				borderColor: "#049E3F",
			},
		},
		clicker: {
			width: "100%",
			height: "100%",
			"& polygon": {
				fill: "transparent",
				stroke: "rgb(239,239,239, 0.75)",
				strokeWidth: 1,
				pointerEvents: "all",
				"&.clickable": {
					cursor: "pointer",
				},
			},
		},
	})
);

export default useStyles;
