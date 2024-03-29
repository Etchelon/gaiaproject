import { Theme } from "@mui/material/styles";

import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';

const BOOSTER_TO_BOARD_RATIO = 16;

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			"&.with-frame": {
				padding: theme.spacing(2),
				[theme.breakpoints.down('md')]: {
					padding: theme.spacing(1),
					borderWidth: 2,
				},
				[theme.breakpoints.down('sm')]: {
					padding: theme.spacing(0.5),
					borderWidth: 1,
				},
				borderWidth: 3,
				borderStyle: "solid",
				borderColor: "lightgray",
				borderRadius: 5,
			},
		},
		upper: {
			display: "flex",
			alignItems: "stretch",
			"& > .spacer": {
				flex: "0 0 auto",
				width: theme.spacing(2),
				[theme.breakpoints.down('md')]: {
					width: theme.spacing(1),
				},
				[theme.breakpoints.down('sm')]: {
					width: theme.spacing(0.5),
				},
			},
		},
		board: {
			flex: "0 1 auto",
			width: `calc(100% * ${BOOSTER_TO_BOARD_RATIO - 1} / ${BOOSTER_TO_BOARD_RATIO})`,
		},
		boosterAndFederations: {
			flex: "0 0 auto",
			width: `calc(100% / ${BOOSTER_TO_BOARD_RATIO})`,
			display: "flex",
			flexDirection: "column",
		},
		booster: {
			width: "100%",
			marginBottom: theme.spacing(2),
			[theme.breakpoints.down('md')]: {
				marginBottom: theme.spacing(1),
			},
		},
		federations: {
			width: "100%",
			display: "flex",
			flexFlow: "row wrap",
			justifyContent: "center",
		},
		federation: {
			width: "75%",
			marginBottom: theme.spacing(1),
			"&:last-child": {
				marginBottom: 0,
			},
		},
		techTiles: {
			display: "flex",
			flexFlow: "row wrap",
			marginTop: theme.spacing(2),
			[theme.breakpoints.down('md')]: {
				marginTop: theme.spacing(1),
			},
		},
		techTile: {
			width: "10%",
			marginRight: theme.spacing(2),
			[theme.breakpoints.down('md')]: {
				marginRight: theme.spacing(1),
			},
			"&:last-child": {
				marginRight: 0,
			},
		},
	})
);

export default useStyles;
