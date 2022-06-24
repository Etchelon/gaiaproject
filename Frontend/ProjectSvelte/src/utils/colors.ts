/**
 * Converts a color from CSS hex format to CSS rgb format.
 * @param {string} color - Hex color, i.e. #nnn or #nnnnnn
 * @returns {string} A CSS rgb color string
 */
function hexToRgb(color: string): string {
	color = color.substring(1);
	const re = new RegExp(`.{1,${color.length >= 6 ? 2 : 1}}`, "g");
	let colors = color.match(re);

	if (colors && colors[0].length === 1) {
		colors = colors.map(n => n + n);
	}

	return colors
		? `rgb${colors.length === 4 ? "a" : ""}(${colors
				.map((n, index) => {
					return index < 3 ? parseInt(n, 16) : Math.round((parseInt(n, 16) / 255) * 1000) / 1000;
				})
				.join(", ")})`
		: "";
}

/**
 * Converts a color object with type and values to a string.
 * @param {object} color - Decomposed color
 * @param {string} color.type - One of: 'rgb', 'rgba', 'hsl', 'hsla'
 * @param {array} color.values - [n,n,n] or [n,n,n,n]
 * @returns {string} A CSS color string
 */
function recomposeColor(color: any) {
	const { type, colorSpace } = color;
	let { values } = color;

	if (type.indexOf("rgb") !== -1) {
		// Only convert the first 3 values to int (i.e. not alpha)
		values = values.map((n: string, i: number) => (i < 3 ? parseInt(n, 10) : n));
	} else if (type.indexOf("hsl") !== -1) {
		values[1] = `${values[1]}%`;
		values[2] = `${values[2]}%`;
	}

	if (type.indexOf("color") !== -1) {
		values = `${colorSpace} ${values.join(" ")}`;
	} else {
		values = `${values.join(", ")}`;
	}

	return `${type}(${values})`;
}

/**
 * Converts a color from hsl format to rgb format.
 * @param {string} color - HSL color values
 * @returns {string} rgb color values
 */
function hslToRgb(color: any) {
	color = decomposeColor(color);
	const { values } = color;
	const h = values[0];
	const s = values[1] / 100;
	const l = values[2] / 100;
	const a = s * Math.min(l, 1 - l);

	const f = (n: number, k = (n + h / 30) % 12) => l - a * Math.max(Math.min(k - 3, 9 - k, 1), -1);

	let type = "rgb";
	const rgb = [Math.round(f(0) * 255), Math.round(f(8) * 255), Math.round(f(4) * 255)];

	if (color.type === "hsla") {
		type += "a";
		rgb.push(values[3]);
	}

	return recomposeColor({
		type,
		values: rgb,
	});
}
/**
 * Returns an object with the type and values of a color.
 *
 * Note: Does not support rgb % values.
 * @param {string} color - CSS color, i.e. one of: #nnn, #nnnnnn, rgb(), rgba(), hsl(), hsla()
 * @returns {object} - A MUI color object: {type: string, values: number[]}
 */
function decomposeColor(color: string): any {
	// Idempotent
	if ((color as any).type) {
		return color;
	}

	if (color.charAt(0) === "#") {
		return decomposeColor(hexToRgb(color));
	}

	const marker = color.indexOf("(");
	const type = color.substring(0, marker);

	if (["rgb", "rgba", "hsl", "hsla", "color"].indexOf(type) === -1) {
		throw new Error(`MUI: Unsupported \`${color}\` color.
  The following formats are supported: #nnn, #nnnnnn, rgb(), rgba(), hsl(), hsla(), color().`);
	}

	let values: string | string[] = color.substring(marker + 1, color.length - 1);
	let colorSpace: string = "";

	if (type === "color") {
		values = values.split(" ");
		colorSpace = values.shift()!;

		if (values.length === 4 && values[3].charAt(0) === "/") {
			values[3] = values[3].substr(1);
		}

		if (["srgb", "display-p3", "a98-rgb", "prophoto-rgb", "rec-2020"].indexOf(colorSpace) === -1) {
			throw new Error(
				`MUI: unsupported \`${colorSpace}\` color space.
  The following color spaces are supported: srgb, display-p3, a98-rgb, prophoto-rgb, rec-2020.`
			);
		}
	} else {
		values = values.split(",");
	}

	return {
		type,
		values: values.map(value => parseFloat(value)),
		colorSpace,
	};
}

/**
 * The relative brightness of any point in a color space,
 * normalized to 0 for darkest black and 1 for lightest white.
 *
 * Formula: https://www.w3.org/TR/WCAG20-TECHS/G17.html#G17-tests
 * @param {string} color - CSS color, i.e. one of: #nnn, #nnnnnn, rgb(), rgba(), hsl(), hsla(), color()
 * @returns {number} The relative brightness of the color in the range 0 - 1
 */
function getLuminance(color: any) {
	color = decomposeColor(color);
	let rgb = color.type === "hsl" ? decomposeColor(hslToRgb(color)).values : color.values;
	rgb = rgb.map((val: number) => {
		if (color.type !== "color") {
			val /= 255; // normalized
		}

		return val <= 0.03928 ? val / 12.92 : ((val + 0.055) / 1.055) ** 2.4;
	}); // Truncate at 3 digits

	return Number((0.2126 * rgb[0] + 0.7152 * rgb[1] + 0.0722 * rgb[2]).toFixed(3));
}

/**
 * Calculates the contrast ratio between two colors.
 *
 * Formula: https://www.w3.org/TR/WCAG20-TECHS/G17.html#G17-tests
 * @param {string} foreground - CSS color, i.e. one of: #nnn, #nnnnnn, rgb(), rgba(), hsl(), hsla()
 * @param {string} background - CSS color, i.e. one of: #nnn, #nnnnnn, rgb(), rgba(), hsl(), hsla()
 * @returns {number} A contrast ratio value in the range 0 - 21.
 */
function getContrastRatio(foreground: any, background: any) {
	const lumA = getLuminance(foreground);
	const lumB = getLuminance(background);
	return (Math.max(lumA, lumB) + 0.05) / (Math.min(lumA, lumB) + 0.05);
}

const contrastThreshold = 3;

export function getContrastColor(background: any) {
	return getContrastRatio(background, "#fff") >= contrastThreshold ? "#fff" : "#000";
}
