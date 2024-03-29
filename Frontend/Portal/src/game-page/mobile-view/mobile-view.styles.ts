import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexColumn, fillParent, fillParentAbs } from "../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		boardArea: {
			...fillParent,
			height: "auto",
			paddingBottom: 3,
			overflow: "auto hidden",
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
		spacer: {
			width: "100%",
			marginTop: theme.spacing(1),
		},
	})
);

export default useStyles;
