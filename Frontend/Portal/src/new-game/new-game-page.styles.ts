import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { centeredFlexRow, responsivePadding } from "../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		loader: {
			display: "flex",
			justifyContent: "center",
			alignItems: "flex-start",
			width: "100%",
			paddingTop: theme.spacing(3),
		},
		wrapper: {
			...responsivePadding(theme),
			overflow: "hidden auto",
		},
		header: {
			marginBottom: theme.spacing(1),
		},
		marginTop: {
			marginTop: theme.spacing(2),
		},
		selectedUser: {
			...centeredFlexRow,
			justifyContent: "flex-start",
			...responsivePadding(theme),
			"& > .username": {
				margin: theme.spacing(0, 2),
				flex: "1 1 auto",
			},
			"&:not(:last-child)": {
				marginBottom: theme.spacing(2),
			},
			"& > .removeBtn": {
				marginLeft: theme.spacing(2),
			},
		},
		actions: {
			...centeredFlexRow,
			justifyContent: "flex-end",
			"& > *": {
				marginLeft: theme.spacing(2),
			},
		},
	})
);

export default useStyles;
