import { Typography, useTheme } from "@material-ui/core";
import Badge from "@material-ui/core/Badge";
import CircularProgress from "@material-ui/core/CircularProgress";
import List from "@material-ui/core/List";
import Popover from "@material-ui/core/Popover";
import NotificationsActiveIcon from "@material-ui/icons/NotificationsActive";
import NotificationsNoneIcon from "@material-ui/icons/NotificationsNone";
import { parseISO } from "date-fns";
import _ from "lodash";
import { MouseEvent, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { NotificationType } from "../../dto/enums";
import { GameNotificationDto } from "../../dto/interfaces";
import { usePageActivation } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import {
	countUnreadNotifications,
	fetchNotifications,
	selectUnreadNotificationsCount,
	selectUserNotifications,
	selectUserNotificationsState,
	setNotificationRead,
} from "../store/active-user.slice";
import GameNotification from "./GameNotification";
import GenericNotification from "./GenericNotification";
import useStyles from "./notifications.styles";

const MANY_UNREAD_NOTIFICATIONS_THRESHOLD = 10;
const UNREAD_NOTIFICATIONS_MAX_COUNT = 99;
const THIRTY_SECONDS = 30000;
let fetchCountInterval: number | undefined;

const Notifications = () => {
	const theme = useTheme();
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

	const fetchCount = () => dispatch(countUnreadNotifications());
	const loadNotifications = () => {
		dispatch(fetchNotifications({ earlierThan: new Date(), enqueue: false }));
	};
	const loadMoreNotifications = () => {
		const earliestLoadedNotificationIsoDate = _.last(userNotifications)!.timestamp;
		const earliestNotificationDate = parseISO(earliestLoadedNotificationIsoDate);
		dispatch(fetchNotifications({ earlierThan: earliestNotificationDate, enqueue: true }));
	};

	const isPopoverOpen = anchorEl !== null;
	const openPopover = (anchor: HTMLElement) => {
		loadNotifications();
		setAnchorEl(anchor);
	};
	const closePopover = () => {
		setAnchorEl(null);
	};
	const togglePopover = (evt: MouseEvent<HTMLElement>) => {
		if (isPopoverOpen) {
			closePopover();
		} else {
			openPopover(evt.currentTarget);
		}
	};

	const onNotificationClicked = (id: string) => () => {
		closePopover();
		dispatch(setNotificationRead({ id }));
	};

	usePageActivation(
		() => {
			fetchCount();
			window.clearInterval(fetchCountInterval);
			fetchCountInterval = window.setInterval(() => {
				fetchCount();
			}, THIRTY_SECONDS);
		},
		() => {
			window.clearInterval(fetchCountInterval);
			fetchCountInterval = undefined;
		}
	);

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
				<List ref={listRef} dense={true} disablePadding={true}>
					{_.map(userNotifications, n => {
						const isLast = _.last(userNotifications) === n;
						switch (n.type) {
							default:
							case NotificationType.Generic:
								return (
									<GenericNotification
										key={n.id}
										notification={n}
										parentScrollable={listRef.current}
										notificationClicked={onNotificationClicked(n.id)}
										bordered={!isLast}
									/>
								);
							case NotificationType.Game:
								const gameNotification = n as GameNotificationDto;
								return (
									<GameNotification
										key={n.id}
										notification={gameNotification}
										parentScrollable={listRef.current}
										notificationClicked={closePopover}
										bordered={!isLast}
									/>
								);
						}
					})}
					{_.isEmpty(userNotifications) && (
						<Typography variant="h6" className={classes.noUnreadNotifications}>
							Nothing to read!
						</Typography>
					)}
					{!isLoadingNotifications && !_.isEmpty(userNotifications) && (
						<div className={classes.loadMoreNotifications} onClick={loadMoreNotifications}>
							<Typography variant="caption" className="gaia-font">
								Load more...
							</Typography>
						</div>
					)}
					{isLoadingNotifications && <CircularProgress size={theme.spacing(2)}></CircularProgress>}
				</List>
			</Popover>
		</div>
	);
};

export default Notifications;
