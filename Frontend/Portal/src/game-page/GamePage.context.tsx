import { createContext, FC, useContext, useState } from "react";
import { useAppContext } from "../global";
import { GamePageViewModel } from "./store/game-page.vm";

interface IGamePageContext {
	vm: GamePageViewModel;
}

const GamePageContext = createContext<IGamePageContext>({} as any);
export const useGamePageContext = () => useContext(GamePageContext);

export const GamePageProvider: FC = ({ children }) => {
	const { httpClient } = useAppContext();
	const [vm] = useState(new GamePageViewModel(httpClient));

	return <GamePageContext.Provider value={{ vm }}>{children}</GamePageContext.Provider>;
};
