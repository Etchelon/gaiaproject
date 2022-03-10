import { FC } from "react";
import AppFrame from "./AppFrame";
import { AppFrameProvider } from "./AppFrame.context";

const AppFrameRoot: FC = ({ children }) => (
	<AppFrameProvider>
		<AppFrame>{children}</AppFrame>
	</AppFrameProvider>
);

export default AppFrameRoot;
