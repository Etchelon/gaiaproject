import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexRow } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			padding: theme.spacing(0, 2, 2),
			maxHeight: "75vh",
		},
		incomes: {
			height: "100%",
			overflow: "hidden auto",
		},
		income: {
			width: "100%",
			marginTop: theme.spacing(1),
			padding: theme.spacing(1, 1.5),
			fontSize: "0.8rem",
		},
		status: {
			display: "flex",
			flexDirection: "column",
			alignItems: "stretch",
			height: "100%",
		},
		playerData: {
			marginBottom: theme.spacing(2),
			height: "auto",
		},
		statusRow: {
			display: "flex",
			alignItems: "flex-start",
			justifyContent: "space-between",
			height: "auto",
			marginTop: theme.spacing(1),
			"& span.gaia-font": {
				fontSize: "1rem",
			},
		},
		resource: {
			display: "flex",
			flexDirection: "column",
			alignItems: "center",
			minWidth: 100,
		},
		indicator: {
			...centeredFlexRow,
			"& > div": {
				width: 35,
				marginRight: theme.spacing(1),
			},
			"& > span": {
				fontSize: "0.75rem",
			},
		},
		commands: {
			marginTop: "auto",
			display: "flex",
			justifyContent: "flex-end",
			"& > .command": {
				marginLeft: theme.spacing(1),
				padding: theme.spacing(0.5, 1.5),
				fontSize: "0.8rem",
			},
		},
	})
);

export default useStyles;
