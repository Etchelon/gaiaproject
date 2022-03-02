import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { flexRow, fillParent } from "../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...fillParent,
			padding: theme.spacing(1),
			[theme.breakpoints.up("md")]: {
				padding: theme.spacing(2),
			},
			overflow: "hidden auto",
			backgroundColor: "black",
		},
		notes: {
			width: "100%",
			maxWidth: "100%",
			marginTop: theme.spacing(1),
		},
		notesHeader: {
			...flexRow,
			"& .spacer": {
				width: theme.spacing(2),
			},
		},
	})
);

export default useStyles;
