import Avatar from "@mui/material/Avatar";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemAvatar from "@mui/material/ListItemAvatar";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import useMediaQuery from "@mui/material/useMediaQuery";
import DoubleArrowIcon from "@mui/icons-material/DoubleArrow";
import SettingsIcon from "@mui/icons-material/Settings";
import placeholder from "../../../assets/person.png";
import { UserInfoDto } from "../../../dto/interfaces";
import ListItemLink from "../../../utils/ListItemLink";
import { Nullable } from "../../../utils/miscellanea";
import AuthButton from "../../AuthButton";
import { useAppFrameContext } from "../../AppFrame.context";
import { observer } from "mobx-react";

const useStyles = makeStyles(() => {
	return createStyles({
		root: {
			display: "flex",
			flexDirection: "column",
			alignItems: "center",
		},
		userActions: {
			alignSelf: "stretch",
		},
	});
});

interface UserBoxProps {
	user: Nullable<UserInfoDto>;
}

const UserBox = ({ user }: UserBoxProps) => {
	const classes = useStyles();
	const useMobileLayout = useMediaQuery("(max-width: 600px)");
	const { vm } = useAppFrameContext();
	const isAuthenticated = user !== null;

	return (
		<div className={classes.root}>
			<List className={classes.userActions}>
				<ListItem>
					<ListItemAvatar>
						<Avatar src={user?.avatar ?? placeholder} />
					</ListItemAvatar>
					<ListItemText primary={user?.username}></ListItemText>
				</ListItem>
				<AuthButton />
				{isAuthenticated && <ListItemLink icon={<SettingsIcon />} primary="Manage profile" to="/profile" />}
				{!useMobileLayout && (
					<li>
						<ListItem button onClick={() => vm.toggleDrawer()}>
							<ListItemIcon>
								<DoubleArrowIcon style={{ transform: vm.isDrawerOpen ? "rotateZ(180deg)" : "" }} />
							</ListItemIcon>
							<ListItemText
								primary={vm.isDrawerOpen ? "Show less" : "Show more"}
								primaryTypographyProps={{ className: "gaia-font", style: { fontSize: "0.8rem" } }}
							/>
						</ListItem>
					</li>
				)}
			</List>
		</div>
	);
};

export default observer(UserBox);
