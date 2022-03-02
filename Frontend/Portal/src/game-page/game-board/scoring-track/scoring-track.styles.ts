import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { ElementSize } from "../../../utils/hooks";

export const WIDTH_TO_HEIGHT_RATIO = 0.962;

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		scoringTrack: {
			position: "relative",
			width: "100%",
			height: ({ height }: ElementSize) => height,
			backgroundColor: "black",
			[theme.breakpoints.down('md')]: {
				borderWidth: 2,
			},
			[theme.breakpoints.down('sm')]: {
				borderWidth: 1,
			},
			borderWidth: 3,
			borderStyle: "solid",
			borderColor: "lightgray",
			borderRadius: 10,
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
