import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
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
		noUnreadNotifications: {
			padding: theme.spacing(2),
		},
	})
);

export default useStyles;
