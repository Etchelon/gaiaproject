import { useRef } from "react";
import ScoringTrackImg from "../../../assets/Resources/Boards/ScoreTrack.png";
import { ScoringTrackDto } from "../../../dto/interfaces";
import { ElementSize, useContainerDimensions } from "../../../utils/hooks";
import FinalScoringTrack from "./FinalScoringTrack";
import RoundScoringTile from "./RoundScoringTile";
import useStyles, { WIDTH_TO_HEIGHT_RATIO } from "./scoring-track.styles";

const roundTileCoordinates = new Map<number, { top: number; left: number; rotation: number }>([
	[1, { top: 0.255, left: 0.075, rotation: -74 }],
	[2, { top: 0.115, left: 0.159, rotation: -43 }],
	[3, { top: 0.038, left: 0.305, rotation: -14 }],
	[4, { top: 0.04, left: 0.4725, rotation: 15 }],
	[5, { top: 0.12, left: 0.618, rotation: 45 }],
	[6, { top: 0.26, left: 0.7, rotation: 75 }],
]);

const finalScoringTrackCoordinates = new Map<number, { top: number; left: number }>([
	[0, { top: 0.495, left: 0.06 }],
	[1, { top: 0.73, left: 0.06 }],
]);

interface ScoringTrackProps {
	board: ScoringTrackDto;
}

function calculateHeight(width: number): number {
	return width * WIDTH_TO_HEIGHT_RATIO;
}

const getRoundTileCoordinates = (round: number, width: number) => {
	const coordinates = roundTileCoordinates.get(round)!;
	return { top: coordinates.top * width, left: coordinates.left * width, transform: `rotateZ(${coordinates.rotation}deg)` };
};

const getFinalScoringTrackCoordinates = (index: number, { width, height }: ElementSize) => {
	const coordinates = finalScoringTrackCoordinates.get(index)!;
	return { top: coordinates.top * height, left: coordinates.left * width };
};

const ScoringTrack = (props: ScoringTrackProps) => {
	const ref = useRef<HTMLDivElement>(null);
	const { width } = useContainerDimensions(ref);
	const height = calculateHeight(width);
	const classes = useStyles({ width, height });
	const board = props.board;
	const roundTileWidth = width * 0.225;

	return (
		<div ref={ref} className={classes.scoringTrack}>
			<img className={classes.image} src={ScoringTrackImg} alt="" />
			{board.scoringTiles.map(roundTile => (
				<div key={roundTile.roundNumber} className={classes.roundTile} style={getRoundTileCoordinates(roundTile.roundNumber, width)}>
					<RoundScoringTile tile={roundTile} width={roundTileWidth} />
				</div>
			))}
			{[board.finalScoring1, board.finalScoring2].map((finalScoring, index) => (
				<div key={finalScoring.tileId} className={classes.finalScoring} style={getFinalScoringTrackCoordinates(index, { width, height })}>
					<FinalScoringTrack scoring={finalScoring} />
				</div>
			))}
		</div>
	);
};

export default ScoringTrack;
