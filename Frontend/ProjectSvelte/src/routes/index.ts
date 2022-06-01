import GamePageRoot from "../game-page/index.svelte";
import GamesPageRoot from "../games/index.svelte";
import HomePage from "../HomePage.svelte";
import NotFoundPage from "../NotFoundPage.svelte";

export default {
	"/": HomePage,
	"/games": GamesPageRoot,
	"/game/:id": GamePageRoot,
	"*": NotFoundPage,
};
