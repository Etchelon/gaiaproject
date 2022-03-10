import GamePage from "./GamePage";
import { GamePageProvider } from "./GamePage.context";

const GamePageRoot = () => (
	<GamePageProvider>
		<GamePage />
	</GamePageProvider>
);

export default GamePageRoot;
