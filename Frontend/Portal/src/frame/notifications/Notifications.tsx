import NotificationsActiveIcon from "@mui/icons-material/NotificationsActive";
import NotificationsNoneIcon from "@mui/icons-material/NotificationsNone";
import { Typography, useTheme } from "@mui/material";
import Badge from "@mui/material/Badge";
import CircularProgress from "@mui/material/CircularProgress";
import List from "@mui/material/List";
import Popover from "@mui/material/Popover";
import { parseISO } from "date-fns";
import { last, isEmpty } from "lodash";
import { observer } from "mobx-react";
import { MouseEvent, useRef, useState } from "react";
import { NotificationType } from "../../dto/enums";
import { GameNotificationDto } from "../../dto/interfaces";
import { usePageActivation } from "../../utils/hooks";
import { Nullable } from "../../utils/miscellanea";
import { useAppFrameContext } from "../AppFrame.context";
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
	const { vm } = useAppFrameContext();
	const listRef = useRef<any>();
	const hasUnreadNotifications = vm.unreadNotificationsCount > 0;
	const hasManyUnreadNotifications = vm.unreadNotificationsCount > MANY_UNREAD_NOTIFICATIONS_THRESHOLD;
	const userNotifications = vm.notifications;
	const isLoadingNotifications = vm.notificationsState === "loading";
	const [anchorEl, setAnchorEl] = useState<Nullable<HTMLElement>>(null);

	const fetchCount = () => vm.countUnreadNotifications();
	const loadNotifications = () => {
		vm.fetchNotifications(new Date(), false);
	};
	const loadMoreNotifications = () => {
		const earliestLoadedNotificationIsoDate = last(userNotifications)!.timestamp;
		const earliestNotificationDate = parseISO(earliestLoadedNotificationIsoDate);
		vm.fetchNotifications(earliestNotificationDate, true);
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
		vm.setNotificationRead(id);
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
				badgeContent={vm.unreadNotificationsCount}
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
					{userNotifications.map(n => {
						const isLast = last(userNotifications) === n;
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
					{isEmpty(userNotifications) && (
						<Typography variant="h6" className={classes.noUnreadNotifications}>
							Nothing to read!
						</Typography>
					)}
					{!isLoadingNotifications && !isEmpty(userNotifications) && (
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

export default observer(Notifications);
