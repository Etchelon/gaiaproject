import { BehaviorSubject } from "rxjs";
import { UserInfoDto } from "../dto/interfaces";
import { Nullable } from "./miscellanea";

class UserInfoService {
	private _auth0Id: string | undefined;
	private readonly _userInfo = new BehaviorSubject<UserInfoDto | null>(null);
	userInfo$ = this._userInfo.asObservable();

	store(auth0Id: string, user: UserInfoDto): void {
		const key = auth0Id;
		window.localStorage.setItem(key, JSON.stringify(user));
		this._auth0Id = auth0Id;
		this._userInfo.next(user);
	}

	load(auth0Id: string): Nullable<UserInfoDto> {
		const key = auth0Id;
		const userStr = window.localStorage.getItem(key);
		if (!userStr) {
			return null;
		}
		const user = JSON.parse(userStr) as UserInfoDto;
		return user;
	}

	getCurrentUser(): UserInfoDto | null {
		return this._userInfo.getValue();
	}

	update(user: UserInfoDto): void {
		this.store(this._auth0Id!, user);
	}
}

const userInfoService = new UserInfoService();
export default userInfoService;
