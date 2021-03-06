import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import { Race } from "../../dto/enums";
import { withAspectRatioW } from "../../utils/miscellanea";
import { getRaceColor } from "../../utils/race-utils";

interface PlayerMarkerProps {
	race: Race;
	shape?: "circle" | "octagon";
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...withAspectRatioW(1),
			width: "calc(100% + 2px)",
			borderRadius: "50%",
			border: ({ race }: PlayerMarkerProps) => `1px solid ${theme.palette.getContrastText(getRaceColor(race))}`,
			backgroundColor: ({ race }: PlayerMarkerProps) => getRaceColor(race),
		},
	})
);

const PlayerMarker = (props: PlayerMarkerProps) => {
	const classes = useStyles(props);
	return <div className={classes.root}></div>;
};

export default PlayerMarker;
