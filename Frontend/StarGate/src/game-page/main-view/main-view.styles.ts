import { createStyles, makeStyles } from "@material-ui/core";
import { fillParent, fillParentAbs } from "../../utils/miscellanea";

const BOOSTER_HEIGHT_TO_WIDTH_RATIO = 3;
export const BOOSTER_AND_FEDERATION_WIDTH = 70;
export const BOOSTER_SPACING = 5;
export const FEDERATION_WIDTH = 70;
export const FEDERATION_SPACING = 5;
export const RACE_AVATAR_WIDTH = 50;

const useStyles = makeStyles(theme =>
	createStyles({
		root: {
			position: "relative",
			...fillParent,
			display: "flex",
			justifyContent: "center",
			alignItems: "center",
			overflow: "auto",
		},
		miniMap: {
			position: "absolute",
			zIndex: 0,
			"&:hover": {
				zIndex: 3, // Lantids parasite mines have z-index 2
			},
			"&.zoomable:hover": {
				zoom: 2,
			},
			"& > .click-trap": {
				...fillParentAbs,
				pointerEvents: "all",
			},
		},
		scoringTrack: {
			width: "25%",
			height: "25%",
			top: 0,
			left: 0,
		},
		researchBoard: {
			bottom: 0,
			left: 0,
		},
		boostersAndFederations: {
			top: 0,
			right: 0,
			width: BOOSTER_AND_FEDERATION_WIDTH,
		},
		roundBoosters: {
			width: BOOSTER_AND_FEDERATION_WIDTH,
			display: "flex",
			justifyContent: "flex-end",
			overflow: "hidden",
			transition: "width 0.25s",
			"&:hover": {
				width: ({ nBoosters }: any) => nBoosters * BOOSTER_AND_FEDERATION_WIDTH + (nBoosters - 1) * BOOSTER_SPACING,
			},
			position: "absolute",
			right: 0,
		},
		roundBooster: {
			top: 0,
			width: BOOSTER_AND_FEDERATION_WIDTH,
			position: "absolute",
			"&:first-child": {
				position: "static",
			},
		},
		federations: {
			width: FEDERATION_WIDTH,
			display: "flex",
			justifyContent: "flex-end",
			overflow: "hidden",
			transition: "width 0.25s",
			"&:hover": {
				width: ({ nFederations }: any) => nFederations * FEDERATION_WIDTH + (nFederations - 1) * FEDERATION_SPACING,
			},
			position: "absolute",
			right: 0,
			top: BOOSTER_HEIGHT_TO_WIDTH_RATIO * BOOSTER_AND_FEDERATION_WIDTH + theme.spacing(2),
		},
		federation: {
			bottom: 0,
			width: FEDERATION_WIDTH,
			position: "absolute",
			"&:first-child": {
				position: "static",
			},
		},
		turnOrder: {
			bottom: 0,
			right: 0,
		},
	})
);

export default useStyles;
