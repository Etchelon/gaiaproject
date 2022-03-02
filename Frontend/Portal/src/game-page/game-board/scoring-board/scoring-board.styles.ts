import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { fillParent } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...fillParent,
			alignSelf: "flex-start",
			overflow: "hidden auto",
			[theme.breakpoints.down("sm")]: {
				overflow: "visible",
			},
		},
		roundBoosters: {
			display: "flex",
			flexWrap: "wrap",
			alignItems: "flex-start",
			...fillParent,
			[theme.breakpoints.down("sm")]: {
				marginTop: theme.spacing(1),
			},
			"& > .booster": {
				width: `calc((100% - 4 * ${theme.spacing(2)}px) / 5)`,
				maxWidth: "100px",
				margin: theme.spacing(0, 2, 2, 0),
				[theme.breakpoints.down("sm")]: {
					width: `calc((100% - 6 * ${theme.spacing(0.5)}px) / 7)`,
					margin: theme.spacing(0, 0.5, 0.5, 0),
				},
				"&:last-child": {
					marginRight: 0,
				},
			},
		},
		federationTokens: {
			display: "flex",
			flexWrap: "wrap",
			alignItems: "flex-start",
			...fillParent,
			"& > .stack": {
				minWidth: 75,
				width: "6%",
				maxWidth: 150,
				margin: theme.spacing(0, 3, 3, 0),
				[theme.breakpoints.down("sm")]: {
					minWidth: 50,
					width: "5%",
					maxWidth: 100,
					margin: theme.spacing(0, 1, 1, 0),
				},
			},
		},
	})
);

export default useStyles;
