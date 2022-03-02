import ListItem from "@mui/material/ListItem";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import DoneAllIcon from "@mui/icons-material/DoneAll";
import InfoIcon from "@mui/icons-material/Info";
import _ from "lodash";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { NotificationDto } from "../../dto/interfaces";
import { ScrollableNode, useVisibility } from "../../utils/hooks";
import { prettyTimestamp } from "../../utils/miscellanea";
import { setNotificationRead } from "../store/active-user.slice";

export const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			position: "relative",
			"& .MuiListItemIcon-root": {
				minWidth: 40,
			},
			"&.bordered": {
				borderBottom: "1px solid lightgray",
			},
		},
		readMarker: {
			position: "absolute",
			bottom: theme.spacing(1),
			right: theme.spacing(1),
		},
	})
);

export interface NotificationProps<T extends NotificationDto = NotificationDto> {
	notification: T;
	parentScrollable: ScrollableNode;
	bordered: boolean;
	notificationClicked(...args: any[]): void;
}

const GenericNotification = ({ notification, parentScrollable, bordered, notificationClicked }: NotificationProps) => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const [isVisible, currentRef] = useVisibility<HTMLDivElement>(parentScrollable, 0);
	const isRead = notification.isRead;
	useEffect(() => {
		if (!isVisible) {
			return;
		}
		if (isRead) {
			return;
		}
		_.delay(() => dispatch(setNotificationRead({ id: notification.id })), 1000);
	}, [isRead, isVisible]);

	return (
		<div ref={currentRef} className={classes.root + (bordered ? " bordered" : "")} onClick={notificationClicked}>
			<ListItem>
				<ListItemIcon>
					<InfoIcon />
				</ListItemIcon>
				<ListItemText primary={notification.text} secondary={prettyTimestamp(notification.timestamp)}></ListItemText>
			</ListItem>
			{notification.isRead && (
				<div className={classes.readMarker}>
					<DoneAllIcon color="secondary" />
				</div>
			)}
		</div>
	);
};

export default GenericNotification;
