import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { Race } from "../../../dto/enums";
import { fillParent, fillParentAbs, withAspectRatioW } from "../../../utils/miscellanea";

const WIDTH_TO_HEIGHT_RATIO = 1.577;

//#region Items geometry

interface BuildingGeometry {
	w: number;
	h: number;
	x: number;
	y: number;
	spacing?: number;
}

const resourcesH = 0.035;
export const buildingGeometries: { [type: string]: BuildingGeometry } = {
	mine: {
		w: 0.0381,
		h: 0.085,
		x: 0.1315,
		y: 0.865,
		spacing: 0.0502,
	},
	ts: {
		w: 0.0381,
		h: 0.0984,
		x: 0.1315,
		y: 0.68,
		spacing: 0.0502,
	},
	rl: {
		w: 0.0623,
		h: 0.0984,
		x: 0.4758,
		y: 0.68,
		spacing: 0.0727,
	},
	pi: {
		w: 0.083,
		h: 0.1311,
		x: 0.1384,
		y: 0.459,
	},
	piBescods: {
		w: 0.083,
		h: 0.1311,
		x: 0.45,
		y: 0.459,
	},
	piAsAmbas: {
		w: 0.08,
		h: 0.098,
		x: 0.235,
		y: 0.4875,
	},
	piAsFiraks: {
		w: 0.08,
		h: 0.098,
		x: 0.2075,
		y: 0.49,
	},
	piAsIvits: {
		w: 0.07,
		h: 0.098,
		x: 0.2375,
		y: 0.48,
	},
	acl: {
		w: 0.0692,
		h: 0.153,
		x: 0.4814,
		y: 0.4481,
	},
	aclBescods: {
		w: 0.0692,
		h: 0.153,
		x: 0.11,
		y: 0.4481,
	},
	acr: {
		w: 0.0692,
		h: 0.153,
		x: 0.5955,
		y: 0.4481,
	},
	acrBescods: {
		w: 0.0692,
		h: 0.153,
		x: 0.22,
		y: 0.4481,
	},
	acrAs: {
		w: 0.06,
		h: 0.098,
		x: 0.61,
		y: 0.5,
	},
	acrAsBescods: {
		w: 0.06,
		h: 0.098,
		x: 0.24,
		y: 0.5,
	},
	gf: {
		w: 0.0623,
		h: 0.0984,
		x: 0.775,
		y: 0.3,
		spacing: 0.0775,
	},
	raceAsBescods: {
		w: 0.07,
		h: 0.098,
		x: 0.885,
		y: 0.1675,
	},
};
export const gfgaX = 0.01;
export const gfgaY = 0.075;
export const gfgaSpacing = 0.085;

//#endregion

const boardElementClass = (type: string) => ({
	width: `${buildingGeometries[type].w * 100}%`,
	height: `${buildingGeometries[type].h * 100}%`,
	top: `${buildingGeometries[type].y * 100}%`,
	left: `${buildingGeometries[type].x * 100}%`,
});

const useStyles = (props: { race: Race }) =>
	makeStyles(() =>
		createStyles({
			root: {
				...withAspectRatioW(WIDTH_TO_HEIGHT_RATIO),
			},
			wrapper: {
				...fillParentAbs,
				display: "flex",
				justifyContent: "center",
				alignItems: "center",
				"& > .building": {
					position: "absolute",
				},
				"& > .actionSpace": {
					position: "absolute",
				},
				"& > .powerToken": {
					position: "absolute",
					width: "3%",
				},
			},
			image: {
				...fillParent,
			},
			resourceTokens: {
				position: "absolute",
				top: 0,
				left: 0,
				width: "100%",
				paddingTop: `${resourcesH * 100}%`,
				"& > .token": {
					position: "absolute",
					top: "1%",
					width: "3%",
					paddingTop: "3%",
					"& > *": {
						...fillParentAbs,
					},
				},
			},
			pi: boardElementClass("pi"),
			piBescods: boardElementClass("piBescods"),
			piAs: boardElementClass(props.race === Race.Ambas ? "piAsAmbas" : props.race === Race.Firaks ? "piAsFiraks" : "piAsIvits"),
			acl: boardElementClass("acl"),
			aclBescods: boardElementClass("aclBescods"),
			acr: boardElementClass("acr"),
			acrBescods: boardElementClass("acrBescods"),
			acrAs: boardElementClass("acrAs"),
			acrAsBescods: boardElementClass("acrAsBescods"),
			ts: boardElementClass("ts"),
			rl: boardElementClass("rl"),
			mine: boardElementClass("mine"),
			gf: boardElementClass("gf"),
			raceAsBescods: boardElementClass("raceAsBescods"),
		})
	);

export default useStyles;
