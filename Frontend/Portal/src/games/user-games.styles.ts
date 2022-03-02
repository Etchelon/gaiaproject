import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { centeredFlexRow } from "../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		loader: {
			display: "flex",
			justifyContent: "center",
			alignItems: "flex-start",
			width: "100%",
			paddingTop: theme.spacing(3),
		},
		wrapper: {
			height: "100%",
			padding: theme.spacing(2),
			[theme.breakpoints.down("sm")]: {
				padding: theme.spacing(1),
			},
			position: "relative",
			overflow: "hidden auto",
		},
		header: {
			...centeredFlexRow,
		},
		spacer: {
			marginRight: "auto",
		},
		actions: {
			...centeredFlexRow,
			marginLeft: theme.spacing(1),
		},
		games: {
			marginTop: theme.spacing(2),
			"& > .game:not(:last-child)": {
				marginBottom: theme.spacing(1),
			},
		},
		newGame: {
			position: "fixed",
			right: theme.spacing(2),
			bottom: theme.spacing(2),
		},
	})
);

export default useStyles;
