import { useAuth0 } from "@auth0/auth0-react";
import MenuIcon from "@mui/icons-material/Menu";
import { useMediaQuery } from "@mui/material";
import AppBar from "@mui/material/AppBar";
import CssBaseline from "@mui/material/CssBaseline";
import Drawer from "@mui/material/Drawer";
import IconButton from "@mui/material/IconButton";
import { useTheme } from "@mui/material/styles";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import { FC, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAppFrameContext } from "./AppFrame.context";
import AppBarImg from "../assets/Resources/splash.jpg";
import navigationService from "../utils/navigation.service";
import useStyles from "./appFrame.styles";
import AppDrawer from "./drawer/AppDrawer";
import Notifications from "./notifications/Notifications";
import { observer } from "mobx-react";

const AppFrame: FC = ({ children }) => {
	const navigate = useNavigate();
	const classes = useStyles();
	const theme = useTheme();
	const useMobileLayout = useMediaQuery("(max-width: 600px)");
	const { vm } = useAppFrameContext();
	const [mobileOpen, setMobileOpen] = useState(false);
	const { isAuthenticated } = useAuth0();

	const handleDrawerToggle = () => {
		setMobileOpen(!mobileOpen);
	};

	useEffect(() => {
		!isAuthenticated && setMobileOpen(false);
	}, [isAuthenticated]);

	useEffect(() => {
		const sub = navigationService.navigate$.subscribe({ next: navigate });
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
					<IconButton color="inherit" aria-label="open drawer" edge="start" onClick={handleDrawerToggle} className={classes.menuButton} size="large">
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
					className={`${classes.drawer} ${vm.isDrawerOpen ? classes.drawerOpen : classes.drawerClose}`}
					classes={{
						paper: vm.isDrawerOpen ? classes.drawerOpen : classes.drawerClose,
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

export default observer(AppFrame);
