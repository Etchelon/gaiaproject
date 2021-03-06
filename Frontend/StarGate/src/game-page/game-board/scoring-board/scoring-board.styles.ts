import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { fillParent } from "../../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...fillParent,
			alignSelf: "flex-start",
			overflow: "hidden auto",
		},
		roundBoosters: {
			display: "flex",
			flexWrap: "wrap",
			alignItems: "flex-start",
			...fillParent,
			"& > .booster": {
				width: `calc((100% - 4 * ${theme.spacing(2)}px) / 5)`,
				maxWidth: "100px",
				margin: theme.spacing(0, 2, 2, 0),
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
					margin: theme.spacing(0, 2, 2, 0),
				},
			},
		},
	})
);

export default useStyles;
