import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexColumn, flexRow } from "../../../utils/miscellanea";

const RACE_AVATAR_WIDTH = 50;

const useStyles = makeStyles(theme =>
	createStyles({
		root: {
			display: "flex",
			padding: theme.spacing(1),
			backgroundColor: "black",
			borderWidth: 3,
			borderStyle: "solid",
			borderColor: "lightgray",
			borderRadius: 10,
			[theme.breakpoints.down('md')]: {
				borderWidth: 2,
			},
			[theme.breakpoints.down('sm')]: {
				borderWidth: 1,
			},
		},
		playersColumn: {
			...centeredFlexColumn,
		},
		playersRow: {
			...flexRow,
		},
		nextRound: {
			marginLeft: theme.spacing(2),
		},
		ongoingAuction: {
			width: "100%",
		},
		avatar: {
			width: RACE_AVATAR_WIDTH,
			"&.vertical": {
				marginTop: theme.spacing(1),
			},
			"&.horizontal": {
				marginLeft: theme.spacing(2),
			},
		},
	})
);

export default useStyles;
