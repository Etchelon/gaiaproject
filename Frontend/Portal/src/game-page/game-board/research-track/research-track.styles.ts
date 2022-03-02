import { createStyles, makeStyles } from "@material-ui/core/styles";
import { ElementSize } from "../../../utils/hooks";

const useStyles = makeStyles(() =>
	createStyles({
		researchTrack: {
			position: "relative",
			display: "flex",
			flexDirection: "column",
			width: ({ width }: ElementSize) => width,
			height: ({ height }: ElementSize) => height,
		},
		federationToken: {
			width: "32%",
			position: "absolute",
			top: "1%",
			left: "38%",
		},
		lostPlanetToken: {
			position: "absolute",
			top: "1%",
			left: "30%",
			height: "9%",
		},
		advancedTile: {
			width: "78%",
			position: "absolute",
			top: "10.5%",
			left: "19%",
		},
		playerMarkers: {
			display: "flex",
			width: "100%",
			flex: "1 1 100%",
			"& > .player-marker": {
				width: "25%",
				height: "100%",
			},
		},
		standardTiles: {
			flex: "1 0 auto",
			alignSelf: "center",
			width: "95%",
		},
	})
);

export default useStyles;
