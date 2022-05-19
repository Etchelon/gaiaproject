import { writable } from "svelte/store";
import type { UserInfoDto } from "../dto/interfaces";

export const userInfo = writable<UserInfoDto | null>(null);
