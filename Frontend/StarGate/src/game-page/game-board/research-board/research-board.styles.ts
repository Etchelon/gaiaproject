import { createStyles, makeStyles } from "@material-ui/core/styles";
import { ElementSize } from "../../../utils/hooks";

export const HEIGHT_TO_WIDTH_RATIO = 0.932;

const useStyles = makeStyles(() =>
	createStyles({
		researchBoard: {
			position: "relative",
			width: ({ width }: ElementSize) => width,
			height: ({ width }: ElementSize) => width * HEIGHT_TO_WIDTH_RATIO,
			backgroundColor: "#efefef",
		},
		image: {
			width: "100%",
			height: "100%",
			position: "absolute",
			top: 0,
			left: 0,
		},
		tracks: {
			display: "flex",
			width: "100%",
			height: "72.5%",
			paddingLeft: "0.5%",
			position: "absolute",
			top: 0,
			left: 0,
		},
		freeTiles: {
			display: "flex",
			width: "100%",
			height: "14%",
			padding: "0.6%",
			position: "absolute",
			top: "72.5%",
			left: 0,
		},
		freeTile: {
			width: "16%",
		},
		actions: {
			display: "flex",
			width: "100%",
			height: "13.5%",
			padding: "0 2% 0 1.8%",
			position: "absolute",
			top: "86.5%",
			left: 0,
		},
		powerActions: {
			display: "flex",
			width: "64%",
			height: "100%",
		},
		powerAction: {
			width: "calc(100% / 7)",
			height: "100%",
			position: "relative",
		},
		qicActions: {
			display: "flex",
			width: "36%",
			height: "100%",
		},
		qicAction: {
			width: "calc(100% / 3)",
			height: "100%",
			position: "relative",
		},
	})
);

export default useStyles;
