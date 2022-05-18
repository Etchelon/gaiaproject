import { svelte } from "@sveltejs/vite-plugin-svelte";
import sveltePreprocess from "svelte-preprocess";
import { defineConfig } from "vite";

const production = process.env.MODE?.toLowerCase() === "production";

export default defineConfig({
	plugins: [
		svelte({
			compilerOptions: {
				dev: !production,
			},
			preprocess: sveltePreprocess({
				sourceMap: !production,
			}),
		}),
	],
});
