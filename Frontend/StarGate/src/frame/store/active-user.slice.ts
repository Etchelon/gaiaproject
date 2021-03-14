import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { NotificationDto } from "../../dto/interfaces";
import { AppStore } from "../../store/types";
import httpClient from "../../utils/http-client";
import { ActiveUserState } from "./types";

const initialState: ActiveUserState = {
	drawerState: "open",
	unreadNotificationsCount: 0,
	notifications: [],
	notificationsState: "idle",
};

interface FetchNotificationsParams {
	earlierThan: Date;
}

export const countUnreadNotifications = createAsyncThunk("user/unread-notifications-count", async () => await httpClient.get<number>("api/Users/CountUnreadNotifications"));

export const fetchNotifications = createAsyncThunk("user/notifications", async ({ earlierThan }: FetchNotificationsParams) => {
	const isoEarlierThan = earlierThan.toISOString();
	const notifications = (await httpClient.get<NotificationDto[]>(`api/Users/GetUserNotifications?earlierThan=${isoEarlierThan}`)) ?? [];
	return notifications;
});

interface SetNotificationReadParams {
	id: string;
}

export const setNotificationRead = createAsyncThunk("user/notification-read", async ({ id }: SetNotificationReadParams) => {
	await httpClient.put(`api/Users/SetNotificationRead/${id}`);
	return id;
});

const activeUserSlice = createSlice({
	name: "userPreferences",
	initialState,
	reducers: {
		loadUserPreferences: (state, action: PayloadAction<ActiveUserState>) => {
			state.drawerState = action.payload.drawerState;
		},
		setDrawerState: (state, action: PayloadAction<"open" | "close">) => {
			state.drawerState = action.payload;
		},
	},
	extraReducers: {
		[countUnreadNotifications.fulfilled.type]: (state, action: PayloadAction<number>) => {
			state.unreadNotificationsCount = action.payload;
		},
		[fetchNotifications.pending.type]: state => {
			state.notificationsState = "loading";
		},
		[fetchNotifications.fulfilled.type]: (state, action: PayloadAction<NotificationDto[]>) => {
			state.notificationsState = "success";
			state.notifications = action.payload;
		},
		[fetchNotifications.rejected.type]: state => {
			state.notificationsState = "failure";
		},
		[setNotificationRead.fulfilled.type]: (state, action: PayloadAction<string>) => {
			state.unreadNotificationsCount = Math.max(state.unreadNotificationsCount - 1, 0);
			const notification = _.find(state.notifications, n => n.id === action.payload)!;
			notification.isRead = true;
		},
	},
});

export default activeUserSlice.reducer;

export const { loadUserPreferences, setDrawerState } = activeUserSlice.actions;
export const selectUserPreferences = (state: AppStore) => state.activeUser;
export const selectIsDrawerOpen = (state: AppStore) => state.activeUser.drawerState === "open";
export const selectUnreadNotificationsCount = (state: AppStore) => state.activeUser.unreadNotificationsCount;
export const selectUserNotifications = (state: AppStore) => state.activeUser.notifications;
export const selectUserNotificationsState = (state: AppStore) => state.activeUser.notificationsState;
