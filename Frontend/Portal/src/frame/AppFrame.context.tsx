import { createContext, FC, useContext, useState } from "react";
import { useAppContext } from "../global";
import { AppFrameViewModel } from "./store/app-frame.vm";

interface IAppFrameContext {
	vm: AppFrameViewModel;
}

const AppFrameContext = createContext<IAppFrameContext>({} as any);
export const useAppFrameContext = () => useContext(AppFrameContext);

export const AppFrameProvider: FC = ({ children }) => {
	const { httpClient } = useAppContext();
	const [vm] = useState(new AppFrameViewModel(httpClient));

	return <AppFrameContext.Provider value={{ vm }}>{children}</AppFrameContext.Provider>;
};
