import type { CapacitorConfig } from "@capacitor/cli";

const config: CapacitorConfig = {
	appId: "com.gaiaproject.app",
	appName: "StarGate",
	webDir: "dist",
	bundledWebRuntime: false,
	server: {
		cleartext: true,
		hostname: "localhost:3000",
		androidScheme: "https",
		iosScheme: "https",
	},
};

export default config;
