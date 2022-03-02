import { createStyles, makeStyles } from "@material-ui/core/styles";

interface MapSizingProps {
	mapWidth: number;
	mapHeight: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		map: {
			position: "relative",
			width: ({ mapWidth }: MapSizingProps) => mapWidth,
			height: ({ mapHeight }: MapSizingProps) => mapHeight,
			pointerEvents: "none",
		},
	})
);

export default useStyles;
