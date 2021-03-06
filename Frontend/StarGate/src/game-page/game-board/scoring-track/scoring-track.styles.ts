import { createStyles, makeStyles } from "@material-ui/core/styles";
import { ElementSize } from "../../../utils/hooks";

export const WIDTH_TO_HEIGHT_RATIO = 0.962;

const useStyles = makeStyles(() =>
	createStyles({
		scoringTrack: {
			position: "relative",
			width: "100%",
			height: ({ height }: ElementSize) => height,
			backgroundColor: "black",
			border: "3px solid darkgray",
			borderRadius: "10px",
		},
		image: {
			width: "100%",
			height: "100%",
			position: "absolute",
			top: 0,
			left: 0,
		},
		roundTile: {
			position: "absolute",
		},
		finalScoring: {
			position: "absolute",
			width: "81%",
			height: "auto",
		},
	})
);

export default useStyles;
