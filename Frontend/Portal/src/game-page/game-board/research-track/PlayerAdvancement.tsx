import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { Race } from "../../../dto/enums";
import PlayerMarker from "../PlayerMarker";

const percentY = new Map<number, number>([
	[0, 87],
	[1, 75],
	[2, 63],
	[3, 44],
	[4, 32],
	[5, 5],
]);

interface PlayerAdvancementProps {
	raceId: Race;
	steps: number;
	width: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			position: "relative",
			width: "100%",
			height: "100%",
		},
		marker: {
			position: "absolute",
			width: ({ width }: PlayerAdvancementProps) => width * 0.6,
			top: ({ steps }: PlayerAdvancementProps) => `${percentY.get(steps)!}%`,
			left: ({ width }: PlayerAdvancementProps) => `calc(50% - ${(width * 0.6) / 2}px)`,
		},
	})
);

const PlayerAdvancement = (props: PlayerAdvancementProps) => {
	const classes = useStyles(props);
	return (
		<div className={classes.root}>
			<div className={classes.marker}>
				<PlayerMarker race={props.raceId} />
			</div>
		</div>
	);
};

export default PlayerAdvancement;
