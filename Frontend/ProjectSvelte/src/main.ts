import App from "./App.svelte";

const app = new App({
	target: document.body,
	props: {
		name: "Gaia Project",
	},
});

export default app;
