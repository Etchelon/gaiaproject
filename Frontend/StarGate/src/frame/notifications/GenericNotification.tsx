import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import InfoIcon from "@material-ui/icons/Info";
import _ from "lodash";
import { RefObject, useEffect } from "react";
import { useDispatch } from "react-redux";
import { NotificationDto } from "../../dto/interfaces";
import useVisibility, { ScrollableNode } from "../../utils/hooks";
import { prettyTimestamp } from "../../utils/miscellanea";

export interface NotificationProps<T extends NotificationDto = NotificationDto> {
	notification: T;
	parentScrollable: ScrollableNode;
	notificationClicked(...args: any[]): void;
}

const GenericNotification = ({ notification, parentScrollable, notificationClicked }: NotificationProps) => {
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
		console.log(`Notification ${notification.id} is now read`);
	}, [isRead, isVisible]);

	return (
		<div ref={currentRef} onClick={notificationClicked}>
			<ListItem>
				<ListItemIcon>
					<InfoIcon />
				</ListItemIcon>
				<ListItemText primary={notification.text} secondary={prettyTimestamp(notification.timestamp)}></ListItemText>
			</ListItem>
		</div>
	);
};

export default GenericNotification;
