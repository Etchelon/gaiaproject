import { makeAutoObservable, runInAction } from "mobx";
import { NotificationDto } from "../../dto/interfaces";
import { LoadingStatus } from "../../games/store/types";
import { HttpClient } from "../../utils/http-client";
import { ActiveUserState, DrawerState } from "./types";

export class AppFrameViewModel {
	drawerState: DrawerState = "open";
	get isDrawerOpen() {
		return this.drawerState === "open";
	}

	unreadNotificationsCount = 0;
	notifications: NotificationDto[] = [];
	notificationsState: LoadingStatus = "idle";

	constructor(private readonly httpClient: HttpClient) {
		makeAutoObservable(this);
	}

	loadUserPreferences({ drawerState }: Pick<ActiveUserState, "drawerState">) {
		this.drawerState = drawerState;
	}

	toggleDrawer() {
		this.drawerState = this.drawerState === "open" ? "close" : "open";
	}

	async fetchNotifications(earlierThan: Date, enqueue: boolean) {
		this.notificationsState = "loading";
		try {
			const isoEarlierThan = earlierThan.toISOString();
			const notifications = await this.httpClient.get<NotificationDto[]>(`api/Users/GetUserNotifications?earlierThan=${isoEarlierThan}`);
			runInAction(() => {
				this.notifications = enqueue ? [...this.notifications, ...notifications] : notifications;
				this.notificationsState = "success";
			});
		} catch (err) {
			this.notificationsState = "failure";
		}
	}

	async countUnreadNotifications() {
		const count = await this.httpClient.get<number>("api/Users/CountUnreadNotifications");
		runInAction(() => (this.unreadNotificationsCount = count));
	}

	async setNotificationRead(id: string) {
		await this.httpClient.put(`api/Users/SetNotificationRead/${id}`);
		runInAction(() => {
			this.unreadNotificationsCount = Math.max(this.unreadNotificationsCount - 1, 0);
			const notification = this.notifications.find(n => n.id === id)!;
			notification.isRead = true;
		});
	}
}
