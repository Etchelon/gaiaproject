import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { fillParent, fillWindow } from "../utils/miscellanea";

const drawerWidth = 240;
const MOBILE_TOOLBAR_HEIGHT = 56;
const TOOLBAR_HEIGHT = 64;

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			display: "flex",
			...fillWindow,
		},
		drawer: {
			[theme.breakpoints.up("sm")]: {
				flexShrink: 0,
			},
		},
		drawerOpen: {
			width: drawerWidth,
			transition: theme.transitions.create("width", {
				easing: theme.transitions.easing.sharp,
				duration: theme.transitions.duration.enteringScreen,
			}),
			overflowX: "hidden",
		},
		drawerClose: {
			transition: theme.transitions.create("width", {
				easing: theme.transitions.easing.sharp,
				duration: theme.transitions.duration.leavingScreen,
			}),
			overflowX: "hidden",
			width: theme.spacing(9) + 1,
		},
		sectionHeader: {
			padding: theme.spacing(2, 2, 0),
		},
		appBar: {
			// TODO: find constant somewhere in @material-ui's Drawer
			zIndex: 1299,
		},
		menuButton: {
			marginRight: theme.spacing(2),
			[theme.breakpoints.up("sm")]: {
				display: "none",
			},
		},
		appBarImg: {
			height: TOOLBAR_HEIGHT,
			padding: theme.spacing(1, 0),
			objectFit: "contain",
			marginRight: theme.spacing(2),
			[theme.breakpoints.down("sm")]: {
				display: "none",
			},
		},
		// necessary for content to be below app bar
		toolbar: theme.mixins.toolbar,
		appDrawer: {
			height: "100%",
			display: "flex",
			flexDirection: "column",
			"& .MuiListItemIcon-root": {
				minWidth: "unset",
				marginLeft: "8px",
				marginRight: "24px",
			},
			"& .MuiListItemText-root > span": {
				whiteSpace: "nowrap",
				textOverflow: "ellipsis",
				overflow: "hidden",
			},
		},
		divider: {
			marginTop: "auto",
		},
		drawerPaper: {
			width: drawerWidth,
		},
		contentWrapper: {
			flexGrow: 1,
			...fillParent,
			maxWidth: "100%",
		},
		content: {
			flexGrow: 1,
			height: `calc(100% - ${MOBILE_TOOLBAR_HEIGHT}px)`,
			[theme.breakpoints.up("sm")]: {
				height: `calc(100% - ${TOOLBAR_HEIGHT}px)`,
			},
		},
	})
);

export default useStyles;
