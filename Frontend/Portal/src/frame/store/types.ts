import { NotificationDto } from "../../dto/interfaces";
import { LoadingStatus } from "../../games/store/types";

export type DrawerState = "open" | "close";

export interface ActiveUserState {
	drawerState: DrawerState;
	unreadNotificationsCount: number;
	notifications: NotificationDto[];
	notificationsState: LoadingStatus;
}
