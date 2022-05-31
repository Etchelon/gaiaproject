import { svelte } from "@sveltejs/vite-plugin-svelte";
import path from "path";
import sveltePreprocess from "svelte-preprocess";
import { defineConfig } from "vite";

const production = process.env.MODE?.toLowerCase() === "production";

export default defineConfig({
	resolve: {
		alias: {
			$components: path.resolve(__dirname, "./src/components"),
			$dto: path.resolve(__dirname, "./src/dto"),
			$utils: path.resolve(__dirname, "./src/utils"),
		},
	},
	plugins: [
		svelte({
			compilerOptions: {
				dev: !production,
			},
			preprocess: sveltePreprocess({
				sourceMap: !production,
				typescript: true,
				postcss: true,
				scss: true,
			}),
		}),
	],
});
