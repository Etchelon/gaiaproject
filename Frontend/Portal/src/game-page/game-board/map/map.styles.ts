import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';

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
