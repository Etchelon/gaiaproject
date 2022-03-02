import { useAuth0 } from "@auth0/auth0-react";
import GamesIcon from "@mui/icons-material/Games";
import HistoryIcon from "@mui/icons-material/History";
import HomeIcon from "@mui/icons-material/Home";
import Divider from "@mui/material/Divider";
import List from "@mui/material/List";
import _ from "lodash";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useAppFrameContext } from "../AppFrame.context";
import { UserInfoDto } from "../../dto/interfaces";
import ListItemLink from "../../utils/ListItemLink";
import { Nullable } from "../../utils/miscellanea";
import userInfoService from "../../utils/user-info.service";
import useStyles from "../appFrame.styles";
import { DrawerState } from "../store/types";
import UserBox from "./user-box/UserBox";

interface AppSection {
	label: string;
	route: string;
	icon?: JSX.Element;
	protected?: boolean;
}

const APP_SECTIONS: AppSection[] = [
	{ label: "Home", route: "/", icon: <HomeIcon />, protected: true },
	{ label: "Games", route: "/games", icon: <GamesIcon />, protected: true },
	{ label: "History", route: "/history", icon: <HistoryIcon />, protected: true },
];

const storageKey = (userId?: string) => `USER_PREFERENCES_${userId ?? "WTF"}`;

const AppDrawer = () => {
	const classes = useStyles();
	const { isAuthenticated, user: auth0User } = useAuth0();
	const { vm } = useAppFrameContext();
	const [userInfo, setUserInfo] = useState<Nullable<UserInfoDto>>(null);

	useEffect(() => {
		if (!isAuthenticated) {
			return;
		}
		if (!auth0User) {
			throw new Error("How can we be authenticated without an actual user instance?");
		}

		const userPreferencesStr = window.localStorage.getItem(storageKey(auth0User.sub));
		if (userPreferencesStr) {
			const userPreferences_ = JSON.parse(userPreferencesStr) as { drawerState: DrawerState };
			vm.loadUserPreferences(userPreferences_);
		}
	}, []);

	useEffect(() => {
		const sub = userInfoService.userInfo$.subscribe(user => setUserInfo(user));
		return () => {
			sub.unsubscribe();
		};
	}, []);

	useEffect(() => {
		if (!auth0User) {
			return;
		}

		const userPreferencesStr = JSON.stringify({ drawerState: vm.drawerState });
		window.localStorage.setItem(storageKey(auth0User.sub), userPreferencesStr);
	}, [auth0User, vm.drawerState]);

	return (
		<div className={classes.appDrawer}>
			<div className={classes.toolbar} />
			<List>
				{_.chain(APP_SECTIONS)
					.filter(section => isAuthenticated || !section.protected)
					.map(section => <ListItemLink button key={section.label} primary={section.label} icon={section.icon} to={section.route} />)
					.value()}
			</List>
			<div className={classes.divider}>
				<Divider />
			</div>
			<UserBox user={userInfo} />
		</div>
	);
};

export default observer(AppDrawer);
