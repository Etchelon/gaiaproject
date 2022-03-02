import { createStyles, makeStyles, Theme } from "@material-ui/core";
import { centeredFlexColumn } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: { ...centeredFlexColumn, cursor: "pointer" },
		raceName: {
			marginBottom: theme.spacing(1),
		},
		raceImage: {
			padding: theme.spacing(1),
			position: "relative",
			"& > .check": {
				position: "absolute",
				bottom: -3,
				right: 0,
			},
		},
		playerInfo: {
			...centeredFlexColumn,
			marginTop: theme.spacing(1),
		},
	})
);

export default useStyles;
