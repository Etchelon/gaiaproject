import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { centeredFlexColumn, fillParent, fillParentAbs } from "../utils/miscellanea";

const TABS_HEIGHT = 48;
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
			backgroundColor: "black",
		},
		statusBar: {
			height: STATUSBAR_HEIGHT,
			padding: 3,
		},
		wrapper: {
			height: `calc(100% - ${STATUSBAR_HEIGHT}px)`,
			padding: 3,
			paddingBottom: 0,
			overflow: "hidden",
		},
		boardArea: {
			...fillParent,
		},
		controlArea: {
			...fillParent,
			paddingLeft: 3,
		},
		playerBoxesAndLogs: {
			...fillParent,
			...centeredFlexColumn,
			overflow: "hidden auto",
		},
		playerBox: {
			width: "100%",
			height: "auto",
			flex: "0 0 auto",
			marginTop: theme.spacing(1),
			position: "relative",
			"&:first-child": {
				marginTop: 0,
			},
			"& > .hoverTrap": {
				...fillParentAbs,
				pointerEvents: "all",
			},
		},
		gameLog: {
			width: "100%",
			height: "auto",
			flex: "0 0 auto",
			marginTop: theme.spacing(1),
			"&:last-child": {
				marginTop: theme.spacing(2),
			},
		},
		activeViewContainer: {
			width: "100%",
			height: `calc(100% - ${TABS_HEIGHT}px - ${theme.spacing(1)}px)`,
			marginBottom: theme.spacing(1),
			display: "flex",
			justifyContent: "center",
			alignItems: "center",
			position: "relative",
		},
		hoveredPlayerArea: {
			position: "absolute",
			backgroundColor: "black",
			zIndex: 10,
		},
		tabs: {
			width: "100%",
			height: TABS_HEIGHT,
			backgroundColor: "white",
			color: "black",
			fontSize: "1.5em",
			"& > .MuiBottomNavigationAction-root": {
				maxWidth: "unset",
			},
		},
	})
);

export default useStyles;
