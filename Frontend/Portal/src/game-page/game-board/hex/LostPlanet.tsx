import { createStyles, makeStyles } from "@material-ui/core/styles";
import LostPlanetImg from "../../../assets/Resources/Markers/LostPlanet.png";
import { Race } from "../../../dto/enums";
import Satellite from "./Satellite";

interface LostPlanetProps {
	width: number;
	height: number;
	raceId: Race;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			position: "relative",
			width: "100%",
			height: "100%",
		},
		lostPlanet: {
			position: "absolute",
			width: ({ width }: LostPlanetProps) => width * 0.75,
			height: ({ width }: LostPlanetProps) => width * 0.75,
			top: ({ height }: LostPlanetProps) => (height * 0.25) / 2,
			left: ({ width }: LostPlanetProps) => (width * 0.25) / 2,
		},
		satellite: {
			position: "absolute",
			top: ({ height }: LostPlanetProps) => height * 0.65,
			left: ({ width }: LostPlanetProps) => width * 0.2,
		},
	})
);

const LostPlanet = (props: LostPlanetProps) => {
	const classes = useStyles(props);

	return (
		<div className={classes.root}>
			<img className={classes.lostPlanet} src={LostPlanetImg} alt="" />
			<div className={classes.satellite}>
				<Satellite raceId={props.raceId} width={props.height / 4} />
			</div>
		</div>
	);
};

export default LostPlanet;
