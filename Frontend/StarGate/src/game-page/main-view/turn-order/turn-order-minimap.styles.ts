import { createStyles, makeStyles } from "@material-ui/core";
import { centeredFlexColumn } from "../../../utils/miscellanea";

const RACE_AVATAR_WIDTH = 50;

const useStyles = makeStyles(theme =>
	createStyles({
		root: {
			display: "flex",
			padding: theme.spacing(1),
			backgroundColor: "black",
			border: "3px solid darkgray",
			borderRadius: "10px",
		},
		roundColumn: {
			...centeredFlexColumn,
		},
		nextRound: {
			marginLeft: theme.spacing(2),
		},
		ongoingAuction: {
			width: "100%",
		},
		avatar: {
			width: RACE_AVATAR_WIDTH,
			marginTop: theme.spacing(1),
		},
	})
);

export default useStyles;
