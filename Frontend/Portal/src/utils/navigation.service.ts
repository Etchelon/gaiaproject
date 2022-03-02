import { Subject } from "rxjs";

class NavigationService {
	private readonly _navigate = new Subject<string>();
	navigate$ = this._navigate.asObservable();

	navigateTo(path: string): void {
		this._navigate.next(path);
	}
}

const navigationService = new NavigationService();
export default navigationService;
