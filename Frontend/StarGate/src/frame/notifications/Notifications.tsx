import Badge from "@material-ui/core/Badge";
import List from "@material-ui/core/List";
import Popover from "@material-ui/core/Popover";
import NotificationsActiveIcon from "@material-ui/icons/NotificationsActive";
import NotificationsNoneIcon from "@material-ui/icons/NotificationsNone";
import _ from "lodash";
import React, { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { NotificationType } from "../../dto/enums";
import { GameNotificationDto } from "../../dto/interfaces";
import { Nullable } from "../../utils/miscellanea";
import { countUnreadNotifications, fetchNotifications, selectUnreadNotificationsCount, selectUserNotifications, selectUserNotificationsState } from "../store/active-user.slice";
import GameNotification from "./GameNotification";
import GenericNotification from "./GenericNotification";
import useStyles from "./notifications.styles";

const MANY_UNREAD_NOTIFICATIONS_THRESHOLD = 10;
const UNREAD_NOTIFICATIONS_MAX_COUNT = 99;

const Notifications = () => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const listRef = useRef<any>();
	const unreadNotificationsCount = useSelector(selectUnreadNotificationsCount);
	const hasUnreadNotifications = unreadNotificationsCount > 0;
	const hasManyUnreadNotifications = unreadNotificationsCount > MANY_UNREAD_NOTIFICATIONS_THRESHOLD;
	const userNotifications = useSelector(selectUserNotifications);
	const userNotificationsState = useSelector(selectUserNotificationsState);
	const isLoadingNotifications = userNotificationsState === "loading";
	const [anchorEl, setAnchorEl] = useState<Nullable<HTMLElement>>(null);
	const isPopoverOpen = anchorEl !== null;
	const openPopover = (anchor: HTMLElement) => {
		dispatch(fetchNotifications({ earlierThan: new Date() }));
		setAnchorEl(anchor);
	};
	const closePopover = () => {
		setAnchorEl(null);
	};
	const togglePopover = (evt: React.MouseEvent<HTMLElement>) => {
		if (isPopoverOpen) {
			closePopover();
		} else {
			openPopover(evt.currentTarget);
		}
	};

	useEffect(() => {
		dispatch(countUnreadNotifications());
	}, []);

	return (
		<div className={classes.root}>
			<Badge
				badgeContent={unreadNotificationsCount}
				showZero={false}
				color={hasManyUnreadNotifications ? "error" : "secondary"}
				max={UNREAD_NOTIFICATIONS_MAX_COUNT}
				onClick={togglePopover}
			>
				{hasUnreadNotifications && <NotificationsActiveIcon />}
				{!hasUnreadNotifications && <NotificationsNoneIcon />}
			</Badge>
			<Popover
				anchorEl={anchorEl}
				open={isPopoverOpen}
				onClose={closePopover}
				anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
				transformOrigin={{ vertical: "top", horizontal: "right" }}
				PaperProps={{ style: { width: 300 } }}
			>
				<List ref={listRef} dense={true}>
					{_.map(userNotifications, n => {
						switch (n.type) {
							default:
							case NotificationType.Generic:
								return <GenericNotification key={n.id} notification={n} parentScrollable={listRef.current} notificationClicked={closePopover} />;
							case NotificationType.Game:
								const gameNotification = n as GameNotificationDto;
								return <GameNotification key={n.id} notification={gameNotification} parentScrollable={listRef.current} notificationClicked={closePopover} />;
						}
					})}
				</List>
			</Popover>
		</div>
	);
};

export default Notifications;
