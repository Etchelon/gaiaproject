const colors = require("tailwindcss/colors");

module.exports = {
	content: ["./src/**/*.{html,css,svelte}"],
	theme: {
		colors: {
			primary: colors.blue,
			secondary: colors.green,
			gray: colors.stone,
			white: colors.white,
			transparent: "transparent",
			current: "currentColor",
		},
		extend: {},
	},
	plugins: [],
};
