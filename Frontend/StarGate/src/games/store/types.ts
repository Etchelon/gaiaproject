import { EntityState } from "@reduxjs/toolkit";
import { GameInfoDto } from "../../dto/interfaces";
import { Nullable } from "../../utils/miscellanea";

export type LoadingStatus = "idle" | "loading" | "success" | "failure";

export interface GamesSliceState extends EntityState<GameInfoDto> {
	status: LoadingStatus;
	lastFetchParams: Nullable<string>;
	lastFetched: Nullable<string>;
	error: Nullable<string>;
}
