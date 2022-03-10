import DoneAllIcon from "@mui/icons-material/DoneAll";
import GamesIcon from "@mui/icons-material/Games";
import { delay } from "lodash";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { GameNotificationDto } from "../../dto/interfaces";
import { useVisibility } from "../../utils/hooks";
import ListItemLink from "../../utils/ListItemLink";
import { prettyTimestamp } from "../../utils/miscellanea";
import { useAppFrameContext } from "../AppFrame.context";
import { NotificationProps, useStyles } from "./GenericNotification";

const GameNotification = ({ notification, parentScrollable, bordered, notificationClicked }: NotificationProps<GameNotificationDto>) => {
	const classes = useStyles();
	const [isVisible, currentRef] = useVisibility<HTMLDivElement>(parentScrollable, 0);
	const { vm } = useAppFrameContext();
	const isRead = notification.isRead;

	useEffect(() => {
		if (!isVisible) {
			return;
		}
		if (isRead) {
			return;
		}

		delay(() => vm.setNotificationRead(notification.id), 1000);
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

export default observer(GameNotification);
