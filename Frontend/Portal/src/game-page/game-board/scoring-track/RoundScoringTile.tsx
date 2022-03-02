import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { RoundScoringTileType } from "../../../dto/enums";
import { RoundScoringTileDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";

const WIDTH_TO_HEIGHT_RATIO = 0.876;

const roundScoringTileNames = new Map<RoundScoringTileType, string>([
	[RoundScoringTileType.PointsPerTerraformingStep2, "RNDter"],
	[RoundScoringTileType.PointsPerResearchStep2, "RNDstp"],
	[RoundScoringTileType.PointsPerMine2, "RNDmin"],
	[RoundScoringTileType.PointsPerTradingStation3, "RNDtrs3"],
	[RoundScoringTileType.PointsPerTradingStation4, "RNDtrs4"],
	[RoundScoringTileType.PointsPerGaiaPlanet3, "RNDgai3"],
	[RoundScoringTileType.PointsPerGaiaPlanet4, "RNDgai4"],
	[RoundScoringTileType.PointsPerBigBuilding5, "RNDpia"],
	[RoundScoringTileType.PointsPerBigBuilding5Bis, "RNDpia"],
	[RoundScoringTileType.PointsPerFederation5, "RNDfed"],
]);

interface RoundScoringTileProps {
	tile: RoundScoringTileDto;
	width: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		scoringTile: {
			width: ({ width }: RoundScoringTileProps) => width,
			height: ({ width }: RoundScoringTileProps) => width / WIDTH_TO_HEIGHT_RATIO,
			opacity: ({ tile }: RoundScoringTileProps) => (tile.inactive ? 0.3 : 1),
		},
		image: {
			width: "100%",
			height: "100%",
		},
	})
);

const RoundScoringTile = (props: RoundScoringTileProps) => {
	const imgUrl = useAssetUrl(`Boards/ScoringTiles/${roundScoringTileNames.get(props.tile.tileId)!}.png`);
	const classes = useStyles(props);

	return (
		<div className={classes.scoringTile}>
			<img className={classes.image} src={imgUrl} alt="" />
		</div>
	);
};

export default RoundScoringTile;
