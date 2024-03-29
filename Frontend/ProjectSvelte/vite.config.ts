import { svelte } from "@sveltejs/vite-plugin-svelte";
import path from "path";
import sveltePreprocess from "svelte-preprocess";
import { defineConfig } from "vite";

type ViteMode = "development" | "production";

export default ({ mode }: { mode: ViteMode }) => {
	const production = mode === "production";
	return defineConfig({
		resolve: {
			alias: {
				$components: path.resolve(__dirname, "./src/components"),
				$dto: path.resolve(__dirname, "./src/dto"),
				$utils: path.resolve(__dirname, "./src/utils"),
			},
		},
		server: {
			https: true,
			host: "0.0.0.0",
			open: false,
			port: 3000,
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
};
