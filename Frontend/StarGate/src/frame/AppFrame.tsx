import { useAuth0 } from "@auth0/auth0-react";
import { useMediaQuery } from "@material-ui/core";
import AppBar from "@material-ui/core/AppBar";
import CssBaseline from "@material-ui/core/CssBaseline";
import Drawer from "@material-ui/core/Drawer";
import IconButton from "@material-ui/core/IconButton";
import { useTheme } from "@material-ui/core/styles";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import MenuIcon from "@material-ui/icons/Menu";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Link, useHistory } from "react-router-dom";
import AppBarImg from "../assets/Resources/splash.jpg";
import navigationService from "../utils/navigation.service";
import useStyles from "./appFrame.styles";
import AppDrawer from "./drawer/AppDrawer";
import Notifications from "./notifications/Notifications";
import { selectIsDrawerOpen } from "./store/active-user.slice";

interface AppFrameProps {
	children: any;
}

const AppFrame = ({ children }: AppFrameProps) => {
	const history = useHistory();
	const classes = useStyles();
	const theme = useTheme();
	const useMobileLayout = useMediaQuery("(max-width: 600px)");
	const isDrawerOpen = useSelector(selectIsDrawerOpen);
	const [mobileOpen, setMobileOpen] = useState(false);
	const { isAuthenticated } = useAuth0();

	const handleDrawerToggle = () => {
		setMobileOpen(!mobileOpen);
	};

	useEffect(() => {
		!isAuthenticated && setMobileOpen(false);
	}, [isAuthenticated]);

	useEffect(() => {
		const sub = navigationService.navigate$.subscribe(path => history.push(path));
		return () => {
			sub.unsubscribe();
		};
	}, []);

	const container = window !== undefined ? () => window.document.body : undefined;

	return (
		<div className={classes.root}>
			<CssBaseline />
			<AppBar position="fixed" className={classes.appBar}>
				<Toolbar variant="dense">
					<IconButton color="inherit" aria-label="open drawer" edge="start" onClick={handleDrawerToggle} className={classes.menuButton}>
						<MenuIcon />
					</IconButton>
					<img className={classes.appBarImg} src={AppBarImg} alt="" />
					<Link to="/" style={{ color: "white", textDecoration: "none" }}>
						<Typography variant="h6" className="gaia-font" noWrap>
							Gaia Project
						</Typography>
					</Link>
					{isAuthenticated && (
						<div className={classes.notifications}>
							<Notifications />
						</div>
					)}
				</Toolbar>
			</AppBar>
			{useMobileLayout && (
				<Drawer
					container={container}
					variant="temporary"
					anchor={theme.direction === "rtl" ? "right" : "left"}
					open={mobileOpen}
					onClose={handleDrawerToggle}
					onClick={() => setMobileOpen(false)}
					classes={{
						paper: classes.drawerPaper,
					}}
					ModalProps={{
						keepMounted: true, // Better open performance on mobile.
					}}
				>
					<AppDrawer />
				</Drawer>
			)}
			{!useMobileLayout && (
				<Drawer
					className={`${classes.drawer} ${isDrawerOpen ? classes.drawerOpen : classes.drawerClose}`}
					classes={{
						paper: isDrawerOpen ? classes.drawerOpen : classes.drawerClose,
					}}
					variant="permanent"
				>
					<AppDrawer />
				</Drawer>
			)}
			<main className={classes.contentWrapper}>
				<div className={classes.toolbar} />
				<div className={classes.content}>{children}</div>
			</main>
		</div>
	);
};

export default AppFrame;
