import { wrap } from "svelte-spa-router/wrap";
import { get } from "svelte/store";
import HomePage from "../HomePage.svelte";
import NotFoundPage from "../NotFoundPage.svelte";
import ProfilePage from "../Profile/index.svelte";
import UnauthorizedPage from "../UnauthorizedPage.svelte";

const authenticationGuard = auth => () => get(auth.isAuthenticated);

const getRoutes = auth => ({
	"/": HomePage,
	"/games/:kind": wrap({
		asyncComponent: () => import("../games/index.svelte"),
		conditions: [authenticationGuard(auth)],
	}),
	"/game/:id": wrap({
		asyncComponent: () => import("../game-page/index.svelte"),
		conditions: [authenticationGuard(auth)],
	}),
	"/profile": ProfilePage,
	"/unauthorized": UnauthorizedPage,
	"*": NotFoundPage,
});

export default getRoutes;
