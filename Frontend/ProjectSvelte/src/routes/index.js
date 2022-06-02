import { wrap } from "svelte-spa-router/wrap";
import HomePage from "../HomePage.svelte";
import NotFoundPage from "../NotFoundPage.svelte";
import UnauthorizedPage from "../UnauthorizedPage.svelte";
import { useAuth0 } from "../auth";
import { get } from "svelte/store";

const authenticationGuard = () => get(useAuth0.isAuthenticated);

export default {
	"/": HomePage,
	"/games": wrap({
		asyncComponent: () => import("../games/index.svelte"),
		conditions: [authenticationGuard],
	}),
	"/game/:id": wrap({
		asyncComponent: () => import("../game-page/index.svelte"),
		conditions: [authenticationGuard],
	}),
	"/unauthorized": UnauthorizedPage,
	"*": NotFoundPage,
};
