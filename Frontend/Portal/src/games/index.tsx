import { GameKind } from "./store/types";
import UserGames from "./UserGames";
import { GamesProvider } from "./UserGames.context";

interface IProps {
	kind: GameKind;
}

const GamesRoot = ({ kind }: IProps) => (
	<GamesProvider>
		<UserGames kind={kind} />
	</GamesProvider>
);

export default GamesRoot;
