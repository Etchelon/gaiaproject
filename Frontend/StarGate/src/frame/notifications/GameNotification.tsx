import GamesIcon from "@material-ui/icons/Games";
import _ from "lodash";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { GameNotificationDto } from "../../dto/interfaces";
import useVisibility from "../../utils/hooks";
import ListItemLink from "../../utils/ListItemLink";
import { prettyTimestamp } from "../../utils/miscellanea";
import { setNotificationRead } from "../store/active-user.slice";
import { NotificationProps } from "./GenericNotification";

const GameNotification = ({ notification, parentScrollable, notificationClicked }: NotificationProps<GameNotificationDto>) => {
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
		dispatch(setNotificationRead({ id: notification.id }));
	}, [isRead, isVisible]);

	return (
		<div ref={currentRef} onClick={notificationClicked}>
			<ListItemLink
				key={notification.id}
				to={`/game/${notification.gameId}`}
				icon={<GamesIcon />}
				primary={notification.text}
				secondary={prettyTimestamp(notification.timestamp)}
			/>
		</div>
	);
};

export default GameNotification;
