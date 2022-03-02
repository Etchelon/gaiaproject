import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
	resolve: {
		alias: {
			"@mui/styled-engine": "@mui/styled-engine-sc",
		},
	},
	plugins: [react()],
});
