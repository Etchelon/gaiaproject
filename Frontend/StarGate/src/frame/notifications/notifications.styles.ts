import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { centeredFlexRow } from "../../utils/miscellanea";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			display: "block",
			cursor: "pointer",
		},
		loadMoreNotifications: {
			...centeredFlexRow,
			marginBottom: theme.spacing(1),
			cursor: "pointer",
		},
	})
);

export default useStyles;
