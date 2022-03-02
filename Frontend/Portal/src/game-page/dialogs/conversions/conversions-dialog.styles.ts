import { Theme } from "@mui/material/styles";

import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			padding: theme.spacing(0, 2, 2),
			maxHeight: "75vh",
		},
		conversions: {
			display: "flex",
		},
		normalConversions: {
			flex: "0 0 50%",
			height: "auto",
			position: "relative",
			"& > img": {
				width: "100%",
				objectFit: "contain",
			},
			"& > :not(img)": {
				position: "absolute",
			},
		},
		spacer: {
			flexShrink: 0,
			width: theme.spacing(1),
		},
		additionalConversions: {
			flex: "0 0 50%",
			height: "100%",
			padding: theme.spacing(0, 1),
			overflow: "hidden auto",
		},
		conversion: {
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
		},
		statusRow: {
			marginBottom: theme.spacing(1),
			"& span.gaia-font": {
				fontSize: "1rem",
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
