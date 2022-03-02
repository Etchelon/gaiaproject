import Grid from "@mui/material/Grid";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import _ from "lodash";
import { PlayerInGameDto } from "../../../dto/interfaces";
import { fillParent } from "../../../utils/miscellanea";
import PlayerArea from "./PlayerArea";

interface PlayerAreasProps {
	players: PlayerInGameDto[];
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			position: "relative",
			...fillParent,
			alignSelf: "flex-start",
			overflow: "hidden auto",
		},
	})
);

const PlayerAreas = ({ players }: PlayerAreasProps) => {
	const classes = useStyles();
	return (
		<div className={classes.root}>
			<Grid container spacing={2}>
				{_.map(players, player => (
					<Grid key={player.id} item xs={12} xl={6}>
						<PlayerArea player={player} framed={true} />
					</Grid>
				))}
			</Grid>
		</div>
	);
};

export default PlayerAreas;
