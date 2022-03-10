import { Theme } from "@mui/material/styles";

import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';

const backgroundZindex = 0;
const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			display: "flex",
			justifyContent: "center",
			alignItems: "center",
			width: "100%",
			height: "100%",
			position: "relative",
		},
		background: {
			position: "absolute",
			top: 0,
			left: 0,
			width: "100%",
			height: "100%",
			zIndex: backgroundZindex,
			objectFit: "cover",
		},
		card: {
			minWidth: "300px",
			maxWidth: "90vw",
			zIndex: backgroundZindex + 1,
			[theme.breakpoints.up("sm")]: {
				width: "400px",
			},
			marginTop: theme.spacing(5),
		},
		formWrapper: {
			display: "flex",
			flexDirection: "column",
			justifyContent: "flex-start",
			alignItems: "center",
			"& > *": {
				width: "100%",
				marginBottom: theme.spacing(2),
			},
		},
		rememberMe: {
			display: "flex",
			justifyContent: "center",
		},
		submitBtn: {
			marginTop: theme.spacing(3),
		},
	})
);

export default useStyles;
