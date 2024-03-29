import { wrap } from "svelte-spa-router/wrap";
import { get } from "svelte/store";
import HomePage from "../home/HomePage.svelte";
import NotFoundPage from "../not-found/NotFoundPage.svelte";
import UnauthorizedPage from "../unauthorized/UnauthorizedPage.svelte";

const authenticationGuard = auth => () => get(auth.isAuthenticated);

const getRoutes = auth => ({
	"/": HomePage,
	"/games": wrap({
		asyncComponent: () => import("../games/index.svelte"),
		conditions: [authenticationGuard(auth)],
	}),
	"/game/:id": wrap({
		asyncComponent: () => import("../game-page/index.svelte"),
	}),
	"/profile": wrap({
		asyncComponent: () => import("../profile/index.svelte"),
		conditions: [authenticationGuard(auth)],
	}),
	"/unauthorized": UnauthorizedPage,
	"*": NotFoundPage,
});

export default getRoutes;
