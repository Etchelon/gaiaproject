import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";

const STATUSBAR_HEIGHT = 56;
const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		loader: {
			display: "flex",
			justifyContent: "center",
			alignItems: "flex-start",
			width: "100%",
			paddingTop: theme.spacing(3),
		},
		root: {
			height: "100%",
			overflow: "auto",
			backgroundColor: "black",
		},
		statusBar: {
			"&.desktop": {
				height: STATUSBAR_HEIGHT,
			},
			"&.mobile": {
				position: "sticky",
				top: 0,
				zIndex: 1,
				backgroundColor: "black",
			},
			padding: 3,
		},
		gameView: {
			padding: 3,
			"&.mobile": {
				overflow: "auto",
			},
			"&.desktop": {
				height: `calc(100% - ${STATUSBAR_HEIGHT}px)`,
				overflow: "hidden",
				paddingBottom: 0,
			},
		},
	})
);

export default useStyles;
