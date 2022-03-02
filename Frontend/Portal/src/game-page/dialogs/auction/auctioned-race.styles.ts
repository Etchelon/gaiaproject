import { Theme } from "@mui/material";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexColumn } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: { ...centeredFlexColumn, cursor: "pointer" },
		raceName: {
			marginBottom: theme.spacing(1),
		},
		raceImage: {
			padding: theme.spacing(1),
			position: "relative",
			"& > .check": {
				position: "absolute",
				bottom: -3,
				right: 0,
			},
		},
		playerInfo: {
			...centeredFlexColumn,
			marginTop: theme.spacing(1),
		},
	})
);

export default useStyles;
