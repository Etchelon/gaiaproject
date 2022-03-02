import { NotificationDto } from "../../dto/interfaces";
import { LoadingStatus } from "../../games/store/types";

export interface ActiveUserState {
	drawerState: "open" | "close";
	unreadNotificationsCount: number;
	notifications: NotificationDto[];
	notificationsState: LoadingStatus;
}
