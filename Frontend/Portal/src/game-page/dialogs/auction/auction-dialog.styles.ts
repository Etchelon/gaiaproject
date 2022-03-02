import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { responsivePadding } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			overflow: "hidden auto",
			...responsivePadding(theme),
			paddingTop: theme.spacing(1),
		},
		header: {
			padding: theme.spacing(1, 0, 2),
			minWidth: 300,
		},
		raceList: {
			display: "flex",
			width: "100%",
			height: "auto",
			overflowX: "scroll",
			overflowY: "hidden",
			scrollbarWidth: "none",
			"&::-webkit-scrollbar": {
				display: "none",
			},
		},
		raceBoard: {
			width: "100%",
			margin: theme.spacing(2, 0),
		},
		marginH: {
			margin: theme.spacing(0, 2),
		},
		spacer: {
			marginRight: theme.spacing(2),
		},
		bidCommands: {
			marginBottom: theme.spacing(2),
			display: "flex",
			justifyContent: "center",
			"& > .command": {
				marginRight: theme.spacing(1),
				padding: theme.spacing(0.5, 1.5),
				fontSize: "0.8rem",
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
