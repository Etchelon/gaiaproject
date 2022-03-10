import { createContext, FC, useContext, useState } from "react";
import { useAppContext } from "../global";
import { GamesViewModel } from "./store/games.vm";

interface IGamesContext {
	vm: GamesViewModel;
}

const GamesContext = createContext<IGamesContext>({} as any);
export const useGamesContext = () => useContext(GamesContext);

export const GamesProvider: FC = ({ children }) => {
	const { httpClient } = useAppContext();
	const [vm] = useState(new GamesViewModel(httpClient));

	return <GamesContext.Provider value={{ vm }}>{children}</GamesContext.Provider>;
};
