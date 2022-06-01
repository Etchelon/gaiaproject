import { HttpClient } from "$utils/http-client";
import { getContext, setContext } from "svelte";

const key = Symbol.for("AppContext");

interface IAppContext {
	http: HttpClient;
}

const bearerTokenFactoryFn = async () =>
	"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IndXNTF0Mk1LeEd1MWJBUzBXSUZoNyJ9.eyJpc3MiOiJodHRwczovL2dhaWFwcm9qZWN0LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJhdXRoMHw1ZmI4NDRmOTUzN2Y5MjAwNzA3NzcxNTUiLCJhdWQiOlsiaHR0cHM6Ly9nYWlhcHJvamVjdC1iYWNrZW5kLm5ldCIsImh0dHBzOi8vZ2FpYXByb2plY3QuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTY1NDA3NjQxMiwiZXhwIjoxNjU0MTYyODEyLCJhenAiOiJWWUpzUUdNMGhWMlg4dEExQ24wV2RIUmNZNlNHSXA4aiIsInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwgcmVhZDp1c2VycyByZWFkOmN1cnJlbnRfdXNlciByZWFkOmdhbWVzIGNyZWF0ZTpnYW1lIHBsYXk6Z2FtZSJ9.UGYcsSjUSFmjITerVUIwQUp-_nTY_E1SwzYTdfW8bkJDkK9ngqAJwiK5jyDkTo2tFzdTEucPb1LL6BlVp9y_9SwmMviQaPWcbFXyAfv1Kt2U3vODpBUGa3AuwpAgkaS0ROpbwPLWMxt2UBu--YVaU3-HjvrQkxtHGIms2QbouM5p6wop4lDK6WMNTBaGW21F8oM0RN0zGZEEsRfGdKpZOtHXjBtxiTXdj4ixXl0_hnd1ydWed8kV29ktUhFhoT0XSA-PLOcKUOL_OGiRZ_L8JGK-CiN_Ba3ooxePuZNtycZQ9gm0oQnhbLqUc82klvqe5_NyXjEQNxoyZQTqJku8uA";

export const getAppContext = () => getContext<IAppContext>(key);

export const setAppContext = () => {
	const http = new HttpClient("https://gaiaproject-no-docker.azurewebsites.net");
	http.setBearerTokenFactory(bearerTokenFactoryFn);
	const ctx: IAppContext = {
		http,
	};
	setContext(key, ctx);
};
