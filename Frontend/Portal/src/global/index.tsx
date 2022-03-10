import { createContext, FC, useContext } from "react";
import { HttpClient } from "../utils/http-client";
import { HubClient } from "../utils/hub-client";

interface IAppContext {
	httpClient: HttpClient;
	hubClient: HubClient;
}

const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
const httpClient = new HttpClient(actualBaseUrl);
const hubClient = new HubClient(actualBaseUrl);
const appContext: IAppContext = { httpClient, hubClient };

const AppContext = createContext<IAppContext>(appContext);

export const useAppContext = () => useContext(AppContext);

export const AppProvider: FC = ({ children }) => {
	return <AppContext.Provider value={appContext}>{children}</AppContext.Provider>;
};
