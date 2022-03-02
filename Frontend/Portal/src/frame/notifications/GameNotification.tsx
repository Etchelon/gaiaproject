import DoneAllIcon from "@material-ui/icons/DoneAll";
import GamesIcon from "@material-ui/icons/Games";
import _ from "lodash";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { GameNotificationDto } from "../../dto/interfaces";
import { useVisibility } from "../../utils/hooks";
import ListItemLink from "../../utils/ListItemLink";
import { prettyTimestamp } from "../../utils/miscellanea";
import { setNotificationRead } from "../store/active-user.slice";
import { NotificationProps, useStyles } from "./GenericNotification";

const GameNotification = ({ notification, parentScrollable, bordered, notificationClicked }: NotificationProps<GameNotificationDto>) => {
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
			<ListItemLink to={`/game/${notification.gameId}`} icon={<GamesIcon />} primary={notification.text} secondary={prettyTimestamp(notification.timestamp)} />
			{notification.isRead && (
				<div className={classes.readMarker}>
					<DoneAllIcon color="secondary" />
				</div>
			)}
		</div>
	);
};

export default GameNotification;
