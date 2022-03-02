import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			padding: theme.spacing(0, 2, 2),
			maxHeight: "75vh",
		},
		conversions: {
			position: "relative",
			"& > img": {
				width: "100%",
				objectFit: "contain",
			},
			"& > :not(img)": {
				position: "absolute",
			},
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
