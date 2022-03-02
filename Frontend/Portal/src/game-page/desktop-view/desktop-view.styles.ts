import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexColumn, fillParent, fillParentAbs } from "../../utils/miscellanea";

const TABS_HEIGHT = 48;
const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			height: "100%",
		},
		activeViewContainer: {
			width: "100%",
			height: `calc(100% - ${TABS_HEIGHT}px - ${theme.spacing(1)})`,
			marginBottom: theme.spacing(1),
			display: "flex",
			justifyContent: "center",
			alignItems: "center",
			position: "relative",
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
