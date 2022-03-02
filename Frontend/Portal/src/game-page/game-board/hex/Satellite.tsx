import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { Race } from "../../../dto/enums";
import { getRaceColor } from "../../../utils/race-utils";

interface SatelliteProps {
	raceId: Race;
	width: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		satellite: {
			width: ({ width }: SatelliteProps) => width,
			height: ({ width }: SatelliteProps) => width,
			backgroundColor: ({ raceId }: SatelliteProps) => getRaceColor(raceId),
			border: "1px solid darkgray",
			borderRadius: 3,
		},
	})
);

const Satellite = (props: SatelliteProps) => {
	const classes = useStyles(props);
	return <div className={classes.satellite}></div>;
};

export default Satellite;
