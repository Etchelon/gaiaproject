import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import _ from "lodash";
import { useRef } from "react";
import { FinalScoringTileType } from "../../../dto/enums";
import { FinalScoringStateDto } from "../../../dto/interfaces";
import { ElementSize, useAssetUrl, useContainerDimensions } from "../../../utils/hooks";
import { fillParent } from "../../../utils/miscellanea";
import PlayerFinalScoringAdvancement from "./PlayerFinalScoringAdvancement";

const WIDTH_TO_HEIGHT_RATIO = 4.85;
const TILE_WIDTH_TO_HEIGHT_RATIO = 1.6;

const finalScoringTileNames = new Map<FinalScoringTileType, string>([
	[FinalScoringTileType.BuildingsInAFederation, "FINfed"],
	[FinalScoringTileType.BuildingsOnTheMap, "FINbld"],
	[FinalScoringTileType.KnownPlanetTypes, "FINtyp"],
	[FinalScoringTileType.GaiaPlanets, "FINgai"],
	[FinalScoringTileType.Sectors, "FINsec"],
	[FinalScoringTileType.Satellites, "FINsat"],
]);

interface FinalScoringTrackProps {
	scoring: FinalScoringStateDto;
}

const useStyles = makeStyles(() =>
	createStyles({
		finalScoringTrack: {
			display: "block",
			width: "100%",
			height: ({ height }: ElementSize) => height,
		},
		wrapper: {
			display: "flex",
			...fillParent,
		},
		playerScorings: {
			height: ({ height }: ElementSize) => height,
			flex: "1 1 auto",
			"& > .player-scoring": {
				height: "25%",
			},
		},
		tile: {
			width: ({ height }: ElementSize) => height * TILE_WIDTH_TO_HEIGHT_RATIO,
			height: "100%",
			flex: "0 0 auto",
			marginLeft: "auto",
			"& > img": {
				...fillParent,
			},
		},
	})
);

const FinalScoringTrack = ({ scoring }: FinalScoringTrackProps) => {
	const ref = useRef<HTMLDivElement>(null);
	const { width } = useContainerDimensions(ref);
	const height = width / WIDTH_TO_HEIGHT_RATIO;
	const scoringTileImg = useAssetUrl(`Boards/FinalScoring/${finalScoringTileNames.get(scoring.tileId)}.png`);
	const classes = useStyles({ width, height });

	return (
		<div ref={ref} className={classes.finalScoringTrack}>
			<div className={classes.wrapper}>
				<div className={classes.playerScorings}>
					{_.map(scoring.players, playerAdvancement => (
						<div key={playerAdvancement.player.id} className="player-scoring">
							<PlayerFinalScoringAdvancement playerStatus={playerAdvancement} />
						</div>
					))}
				</div>
				<div className={classes.tile}>
					<img src={scoringTileImg} alt="" />
				</div>
			</div>
		</div>
	);
};

export default FinalScoringTrack;
