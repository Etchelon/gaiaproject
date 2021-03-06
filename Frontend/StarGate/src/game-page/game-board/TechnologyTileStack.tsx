import { createStyles, makeStyles } from "@material-ui/core/styles";
import { StandardTechnologyTileStackDto } from "../../dto/interfaces";
import StandardTechnologyTile from "./StandardTechnologyTile";

interface TechnologyTileStackProps {
	stack: StandardTechnologyTileStackDto;
	height?: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			position: "relative",
			width: "auto",
			height: "auto",
			"&.invisible": {
				visibility: "hidden",
			},
		},
		counter: {
			position: "absolute",
			bottom: 3,
			right: 1,
			backgroundColor: "white",
			color: "black",
			padding: "1px 3px",
			fontSize: "0.8rem",
			borderRadius: 5,
		},
	})
);

const TechnologyTileStack = ({ stack: tile }: TechnologyTileStackProps) => {
	const classes = useStyles();
	return (
		<div className={classes.root + (tile.remaining > 0 ? "" : " invisible")}>
			<StandardTechnologyTile type={tile.type} />
		</div>
	);
};

export default TechnologyTileStack;
